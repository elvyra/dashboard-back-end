using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.Api.Models;
using Dashboard.Models;
using Dashboard.Services.PortalCrudService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PortalsController : ControllerBase
    {
        private readonly ILogger<PortalsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPortalCrudService _portalService;

        public PortalsController(
            ILogger<PortalsController> logger, 
            IMapper mapper, 
            IPortalCrudService portalService)
        {
            _logger = logger;
            _mapper = mapper;
            _portalService = portalService;
        }

        /// <summary>
        /// Lists all portals in Db (exeption - with status Deleted)
        /// </summary>
        /// <returns>List of portals</returns>
        /// <response code="200">Returns list of portals in Db</response>
        /// <response code="401">Unauthorized</response>
        // GET: api/Portals
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var List = await _portalService.GetAllToDisplayAsync();
            return Ok(List.Select(portal => _mapper.Map<PortalInListViewModel>(portal)));
        }

        /// <summary>
        /// Portal by Id
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns>Portal</returns>
        /// <response code="200">Returns portal by Id</response>
        /// <response code="204">No portal was found with given Id</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Unauthorized</response>
        // GET: api/Portals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var portal = await _portalService.GetPortalByIdAsync(id);

            if (portal == null)
                return NoContent();

            return Ok(_mapper.Map<PortalFullViewModel>(portal));
        }

        /// <summary>
        /// Creates a portal
        /// </summary>
        /// <remarks>
        /// Sample request (with required parameters):
        ///
        ///     POST /portals
        ///     {
        ///        "name": "Test example",
        ///        "url": "http://www.delfi.lt",        
        ///        "type": 0,
        ///        "method": 0,      
        ///        "isActive": true,
        ///        "basicAuth": false,
        ///        "email": "example@email.com",
        ///        "checkInterval": 300,
        ///        "responseTimeThreshold": 10
        ///     }
        ///
        /// </remarks>
        /// <param name="portalViewModel">New portal info</param>
        /// <returns>Created portal</returns>
        /// <response code="201">New portal created</response>
        /// <response code="204">Creation failed</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>  
        // POST: api/Portals
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] [Bind("Name, Type, URL, Parameters, isActive, Email, CheckInterval, ResponseTimeThreshold, Method, BasicAuth, UserName, PasswordHashed")]  PortalFullViewModel portalViewModel)
        {
            var result = await _portalService.CreateNewPortalAsync(_mapper.Map<Portal>(portalViewModel));

            if (result == null)
                return NoContent();
            
            return CreatedAtAction(nameof(Get), _mapper.Map<PortalFullViewModel>(result));
        }

        /// <summary>
        /// Edit portal info
        /// </summary>
        /// <remarks>
        /// Sample request (with required parameters):
        ///
        ///     PUT /portals/{id}
        ///     {
        ///        "id": "id-goes-here",
        ///        "name": "Test example2",
        ///        "url": "http://www.delfi.lt",
        ///        "callParameters": "",
        ///        "type": 0,
        ///        "method": 0,
        ///        "isActive": true,
        ///        "status": 0,
        ///        "basicAuth": false,
        ///        "email": "example@email.com",
        ///        "checkInterval": 300
        ///     }
        ///
        /// </remarks>
        /// <param name="id">Portal Id</param>
        /// <param name="portalViewModel">New portal info</param>
        /// <returns>Edited portal</returns>
        /// <response code="200">Portal info edited</response>
        /// <response code="204">Editing failed</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>  
        // PUT: api/Portals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] [Bind("Id, Name, Type, URL, Parameters, isActive, Email, CheckInterval, ResponseTimeThreshold, Method, BasicAuth, UserName, PasswordHashed")]  PortalFullViewModel portalViewModel)
        {
            if (id != portalViewModel.Id)
                return BadRequest("URL and body passed ID's does not match");

            var result = await _portalService.EditPortalAsync(_mapper.Map<Portal>(portalViewModel));

            if (result == null)
                return NoContent();
            
            return Ok(_mapper.Map<PortalFullViewModel>(result));
        }

        /// <summary>
        /// Portal status is set to Deleted by Id
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns></returns>
        /// <response code="200">Portal status was set to Deleted</response>
        /// <response code="204">Portal not found by Id</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>  
        // DELETE: api/Portals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _portalService.SetAsDeletedAsync(id);

            if (result == null)
                return NoContent();

            return Ok(_mapper.Map<PortalInListViewModel>(result));
        }


        /// <summary>
        /// Portal status is inverted (Active - NotActive)
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns></returns>
        /// <response code="200">Portal status was Inverted</response>
        /// <response code="204">Portal not found by Id</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>  
        // PATCH: api/Portals/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> InvertStatus(Guid id)
        {
            var result = await _portalService.InvertStatusAsync(id);

            if (result == null)
                return NoContent();

            return Ok(_mapper.Map<PortalInListViewModel>(result));
        }

        /// <summary>
        /// Lists all portals with status Deleted
        /// </summary>
        /// <returns>Portals list</returns>
        /// <response code="200">Portal list with status Deleted</response>
        /// <response code="204">Portal list empty or error</response>
        /// <response code="401">Unauthorized</response>
        // GET: api/Portals/DeletedList
        [HttpGet("DeletedList")]

        public async Task<IActionResult> DeletedList()
        {
            var result = await _portalService.GetAllDeletedAsync();

            if (result == null)
                return NoContent();

            return Ok(result.Select(portal => _mapper.Map<PortalInListViewModel>(portal)));
        }

        /// <summary>
        /// Clears portals with status Deleted list
        /// </summary>
        /// <returns>Portals removed list</returns>
        /// <response code="200">Deleted Portal list</response>
        /// <response code="204">No projects deleted (list empty or error)</response>
        /// <response code="401">Unauthorized</response>
        // DELETE: api/Portals/DeletedList
        [HttpDelete("DeletedList")]
        public async Task<IActionResult> ClearDeletedList()
        {
            var result = await _portalService.ClearAllDeletedAsync();

            if (result == null)
                return NoContent();

            return Ok(result.Select(portal => _mapper.Map<PortalInListViewModel>(portal)));
        }

        /// <summary>
        /// Restores Deleted Portal as Not Active
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns></returns>
        /// <response code="200">Portal status was Restored</response>
        /// <response code="204">Portal not found by Id</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>
        // PATCH: api/Portals/Restore/5
        [HttpPatch("Restore/{id}")]
        public async Task<IActionResult> RestoreDeletedPortalToNotActive(Guid id)
        {
            var result = await _portalService.SetAsNotActiveAsync(id);

            if (result == null)
                return NoContent();

            return Ok(_mapper.Map<PortalInListViewModel>(result));
        }

        /// <summary>
        /// Restores Deleted Portals as Not Active
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Portals status was Restored</response>
        /// <response code="204">No projects restored (list empty or error)</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>
        // PATCH: api/Portals/Restore
        [HttpPatch("Restore")]
        public async Task<IActionResult> RestoreDeletedPortalsToNotActive()
        {
            var result = await _portalService.SetDeletedPortalsAsNotActiveAsync();

            if (result == null)
                return NoContent();

            return Ok(result.Select(portal => _mapper.Map<PortalInListViewModel>(portal)));
        }

        /// <summary>
        /// Clears portal with status Deleted by Id
        /// </summary>
        /// <returns>Removed Portal</returns>
        /// <response code="200">Deleted Portal</response>
        /// <response code="204">No projects deleted (list empty or error)</response>
        /// <response code="400">Validation failed</response>  
        /// <response code="401">Unauthorized</response>
        // DELETE: api/Portals/DeletedById/5
        [HttpDelete("DeleteById/{id}")]
        public async Task<IActionResult> ClearDeletedPortalById(Guid id)
        {
            var result = await _portalService.ClearDeletedByIdAsync(id);

            if (result == null)
                return NoContent();

            return Ok(_mapper.Map<PortalInListViewModel>(result));
        }
    }
}
