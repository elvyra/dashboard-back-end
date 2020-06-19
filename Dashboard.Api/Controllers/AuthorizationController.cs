using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Api.SeededData;
using Dashboard.Data;
using Dashboard.Models;
using Dashboard.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Api.Controllers
{
    [Route("User")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IUserAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ITokenGeneratorService _jwtTokenGenerator;
        private readonly DashboardDbContext _context;
        private readonly IOptions<MainUserData> _optionsMainUser;
        private readonly string _jwtTokenSecretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthorizationController(
            IUserAuthService authService,
            IMapper mapper,
            IConfiguration config,
            ITokenGeneratorService jwtTokenGenerator,
            IOptions<JwtTokenSecretKey> secret,
            IOptions<JwtTokenOptions> options,
            DashboardDbContext context, 
            IOptions<MainUserData> optionsMainUser
        )
        {
            _authService = authService;
            _mapper = mapper;
            _config = config;
            _jwtTokenGenerator = jwtTokenGenerator;
            _context = context;
            _optionsMainUser = optionsMainUser;
            _jwtTokenSecretKey = secret.Value.jwtTokenSecretKey;
            _issuer = options.Value.issuer;
            _audience = options.Value.audience;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="userview">User login data</param>
        /// <returns>JWT access token and refresh token</returns>
        /// <response code="200">Succeeded</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">User disabled or login failed</response>
        // POST: /user/login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] [Bind("Email, Password")] UserLoginPostModel userview)
        {
            var login = _mapper.Map<User>(userview);

            if (_authService.IsUserDisabled(login))
            {
                return Unauthorized("User disabled");
            }

            User user = _authService.AuthUser(login);

            if (user == null)
            {
                return Unauthorized("Incorrect email or password");
            }

            var response = await _jwtTokenGenerator.NewAsync(user);

            if (user.Claims != null && user.Claims.Length > 0)
            {
                return Ok(
                    new
                    {
                        Name = user.Name,
                        Surname = user.Surname ?? "",
                        IsAdmin = user.Claims != null && user.Claims.Length > 0,
                        Token = response.token,
                        RefreshToken = response.refreshToken
                    });
            }

            return Ok(
                    new
                    {
                        Name = user.Name,
                        Surname = user.Surname ?? "",
                        Token = response.token,
                        RefreshToken = response.refreshToken
                    });
        }

        /// <summary>
        /// Refresh expired access token
        /// </summary>
        /// <param name="tokens">Expired access token and valid refresh token</param>
        /// <returns>New access and refresh tokens</returns>
        /// <response code="200">Succeeded</response>
        /// <response code="204">New tokens generation failed</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Invalid access or refresh token</response>
        // POST: /user/refresh
        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAsync([FromBody] [Bind("Token, RefreshToken")] RefreshTokenViewModel tokens)
        {
            var principal = GetPrincipalFromExpiredToken(tokens.Token);

            if (principal == null)
            {
                return Unauthorized("Invalid access token");
            }

            var response = await _jwtTokenGenerator.Refresh(principal, tokens.RefreshToken);

            if (response.token == null)
            {
                return Unauthorized("Invalid access token");
            }

            if (response.refreshToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            if (response.token == null && response.refreshToken == null)
            {
                return NoContent();
            }

            return Ok(
                new
                {
                    Token = response.token,
                    RefreshToken = response.refreshToken
                });
        }

        /// <summary>
        /// Logout, deletes refresh token from Db
        /// </summary>
        /// <param name="tokens">Access token and valid refresh token</param>
        /// <returns>Status</returns>
        /// <response code="200">Succeeded</response>
        /// <response code="204">Logging out failed</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Invalid access token</response>
        // POST: /user/refresh
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] [Bind("Token, RefreshToken")] RefreshTokenViewModel tokens)
        {
            var principal = GetPrincipalFromExpiredToken(tokens.Token);

            if (principal == null)
            {
                return Unauthorized("Invalid access token");
            }

            var response = await _jwtTokenGenerator.RemoveRefreshToken(principal, tokens.RefreshToken);

            if (!response)
            {
                return NoContent();
            }

            return Ok();
        }

        /// <summary>
        /// Reset admin from environment variables (email, password, name and surname)
        /// </summary>
        /// <response code="200">Succeeded</response>
        /// <response code="204">Reset failed, check environment variables</response>
        [HttpGet("ResetAdmin")]
        public IActionResult ResetAdmin()
        {
            if (DatabaseInitializer.Initialize(_context, _optionsMainUser))
                return Ok();

            return NoContent();
        }

        /// <summary>
        /// Gets Principal from expired token
        /// </summary>
        /// <param name="token">JWT access token</param>
        /// <returns>ClaimsPrincipal or null, if token is invalid</returns>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSecretKey)),
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}