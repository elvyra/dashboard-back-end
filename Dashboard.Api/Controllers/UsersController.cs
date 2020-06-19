using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using Dashboard.Services.UserCrudServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Api.Controllers
{
    [Route("User")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserCrudService _crudService;
        private readonly ICheckEmailService _checkEmailService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserCrudService crudService,
            ICheckEmailService checkEmailService,
            IMapper mapper
            )
        {
            _crudService = crudService;
            _checkEmailService = checkEmailService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all users from DB
        /// </summary>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        // GET: /User
        [HttpGet]
        public IEnumerable<UserInListViewModel> Get()
        {
            var List = _crudService.GetAll();
            return List.Select(user => _mapper.Map<UserInListViewModel>(user));
        }

        /// <summary>
        /// Gets one user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400">User not found</response>
        // GET: /User/5
        [HttpGet("{id}", Name = "Get")]
        public UserInListViewModel Get(int id)
        {
            return _mapper.Map<UserInListViewModel>(_crudService.GetOneAsync(id).Result);
        }

        /// <summary>
        /// Toggles user property active/inactive
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Succeeded</response>
        /// <response code="400">Failed</response>
        // POST: /User/toggleactive
        [HttpPost("ToggleActive")]
        public void Post([FromBody] int id)
        {
            _crudService.ToggleActiveAsync(id).Wait();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="userview"></param>
        /// <returns></returns>
        /// <response code="200">Succeeded</response>
        /// <response code="400">Failed</response>
        // POST: /user/register
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegisterPostModel userview)
        {
            IActionResult response = BadRequest("Error");

            if (_checkEmailService.IsAlreadyTaken(userview.Email))
                return BadRequest("Email already in use");

            User user = new User
            {
                Email = userview.Email,
                Name = userview.Name,
                Surname = userview.Surname,
                Password = userview.Password,
                IsActive = userview.IsActive
            };

            if (_crudService.CreateUserAsync(user).Result) response = Ok();
            return response;
        }

        /// <summary>
        /// Edit user infomation and/or password
        /// </summary>
        /// <param name="userview"></param>
        /// <returns></returns>
        /// <response code="200">Succeeded</response>
        /// <response code="400">Failed</response>
        // POST: /user/update
        [HttpPost("Update")]
        public IActionResult Update([FromBody] UserUpdatePostModel userview)
        {
            if (_checkEmailService.IsAlreadyTaken(userview.Email) && _crudService.GetOneAsync(userview.UserId).Result.Email != userview.Email)
                return BadRequest("Email already in use");

            IActionResult response = BadRequest("Error");

            User user = new User
            {
                UserId = userview.UserId,
                Email = userview.Email,
                Name = userview.Name,
                Surname = userview.Surname,
                Password = userview.Password,
                IsActive = userview.IsActive
            };

            if (_crudService.UpdateUserAsync(user).Result) response = Ok();
            return response;
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Succeeded</response>
        /// <response code="400">Failed</response>
        // DELETE: /ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0)
                return BadRequest("You have no power here!");
            
            IActionResult response = BadRequest();

            if (_crudService.DeleteUserAsync(id).Result) 
                response = Ok();

            return response;
        }

        /// <summary>
        /// Toggle isAdmin role/claim
        /// </summary>
        /// <param name="id">User Id</param>
        /// <response code="200">Succeeded</response>
        /// <response code="204">Failed</response>
        /// <response code="400">Incorrect Id passed</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Action can be performed only by Admin</response>
        /// <returns></returns>
        [HttpPatch]
        [Authorize(Policy = "Admin")]
        public IActionResult ToggleIsAdmin(int id)
        {
            if (id < 0)
                return BadRequest("You have no power here!");

            // Retrieve action invoker email from JWT token
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var claims = identity.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == "Email").Value;

            bool? result = _crudService.ToggleIsAdmin(id, email).Result;

            if (result == null)
                return BadRequest("Can not toggle own isAdmin claim.");

            if (result== true) 
                return Ok();

            return NoContent();
        }
    }
}
