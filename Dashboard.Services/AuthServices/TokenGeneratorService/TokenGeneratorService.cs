using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Dashboard.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Dashboard.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dashboard.Services.AuthServices
{
    /// <summary>
    /// Token generator
    /// </summary>
    public class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpirationInMinutes;
        private readonly ILogger<TokenGeneratorService> _logger;
        private readonly DashboardDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="secret">JWT Token Secret Key</param>
        /// <param name="options">JWT Token options</param>
        /// <param name="logger">Ilogger</param>
        /// <param name="context">Database</param>
        public TokenGeneratorService(
            IOptions<JwtTokenSecretKey> secret, 
            IOptions<JwtTokenOptions> options, 
            ILogger<TokenGeneratorService> logger, 
            DashboardDbContext context)
        {
            _secret = secret.Value.jwtTokenSecretKey;
            _issuer = options.Value.issuer;
            _audience = options.Value.audience;
            _accessTokenExpirationInMinutes = options.Value.AccessTokenExpirationInMinutes;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Generates new JWT Token
        /// </summary>
        /// <param name="userInfo">User info</param>
        /// <returns>JWT Token</returns>
        public async Task<(string token, string refreshToken)> NewAsync(User userInfo)
        {
            var claims = GetClaims(userInfo);
            var token = GenerateToken(claims);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(userInfo.UserId, refreshToken);
            return (token, refreshToken);
        }

        /// <summary>
        /// Validates and refreshes tokens 
        /// </summary>
        /// <param name="principal">Principals</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Tuple: access token and refresh token</returns>
        public async Task<(string token, string refreshToken)> Refresh(ClaimsPrincipal principal, string refreshToken)
        {
            var userEmail = principal.Claims
                .Where(c => c.Type == "Email")
                .FirstOrDefault()
                .Value;

            if (string.IsNullOrEmpty(userEmail))
                return (null, refreshToken);

            var user = await _context.Users
                .Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null || user.RefreshTokens == null || user.RefreshTokens.Count == 0)
                return (null, refreshToken);

            if (!ValidRefreshToken(user.UserId, refreshToken).Result)
                return ("NoTokenGenerated", null);

            var newJwtToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            try
            {
                var deleted = await DeleteRefreshToken(user.UserId, refreshToken);
                var saved = await SaveRefreshToken(user.UserId, newRefreshToken);
                if (deleted && saved)
                    return (newJwtToken, newRefreshToken);
                return (null, null);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Deletes refresh token from Db 
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <param name="refreshToken">refresh token</param>
        /// <returns>True - refresh token removed succeeded, false - failed</returns>
        public async Task<bool> RemoveRefreshToken(ClaimsPrincipal principal, string refreshToken)
        {
            var userEmail = principal.Claims
                .Where(c => c.Type == "Email")
                .FirstOrDefault()
                .Value;

            if (string.IsNullOrEmpty(userEmail))
                return false;

            var user = await _context.Users
                .Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null || user.RefreshTokens == null || user.RefreshTokens.Count == 0 || 
                !ValidRefreshToken(user.UserId, refreshToken).Result)
                return false;

            try
            {
                var deleted = await DeleteRefreshToken(user.UserId, refreshToken);
                if (deleted)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Validates if user with given email has given refreah token
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Is valid refresh token</returns>
        public async Task<bool> ValidRefreshToken(int userId, string refreshToken)
        {
            var tokenValid = await _context.RefreshTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    string.Equals(refreshToken, t.Token));

            if (tokenValid == null)
                return false;

            return true;
        }


        /// <summary>
        /// Generates JWT token
        /// </summary>
        /// <param name="claims">Claims to add</param>
        /// <returns>JWT token</returns>
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates user claims
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns>Claims array</returns>
        private IEnumerable<Claim> GetClaims(User userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim("Name", userInfo.Name ?? userInfo.Email),
                new Claim("Surname", userInfo.Surname ?? ""),
                new Claim("Email", userInfo.Email)
            };

            if (userInfo.Claims != null && userInfo.Claims.Length > 0)
            {
                foreach (var claim in userInfo.Claims)
                {
                    claims.Add(new Claim(claim, "true"));
                }
            }

            return claims;
        }

        /// <summary>
        /// Generates refresh token
        /// </summary>
        /// <returns>Refresh token</returns>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// Deletes refresh token from DB
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="refreshToken">Refresh token to delete</param>
        /// <returns>Deleting was succesful</returns>
        private async Task<bool> DeleteRefreshToken(int userId, string refreshToken)
        {
            var tokenInDb = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && string.Equals(refreshToken, t.Token));

            if (tokenInDb == null)
                return false;

            try
            {
                _context.Remove(tokenInDb);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        /// <summary>
        /// Saves Refresh token to Db
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="refreshToken">Refresh Token</param>
        /// <returns>Saved refresh token to DB</returns>
        private async Task<bool> SaveRefreshToken(int userId, string refreshToken)
        {
            var userInDb = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(i => i.UserId == userId);

            if (userInDb == null)
                return false;

            try
            {
                var token = new RefreshToken()
                {
                    UserId = userId,
                    Token = refreshToken
                };
                userInDb.RefreshTokens.Add(token);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }
    }
}
