using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Models;
using Dashboard.Api.Models;
using AutoMapper;
using Dashboard.Services.QueryPortalService;
using Microsoft.AspNetCore.Authorization;

namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QueryPortalController : ControllerBase
    {
        private readonly IQueryPortalService _queryPortalService;
        private readonly IMapper _mapper;
        public QueryPortalController(
            IQueryPortalService queryPortalService, 
            IMapper mapper)
        {
            _queryPortalService = queryPortalService;
            _mapper = mapper;
        }

        /// <summary>
        /// Pinging portal by Id
        /// </summary>
        /// <param name="id">Portal Id</param>
        /// <returns>Response</returns>
        /// <response code="200">Response</response>
        /// <response code="204">Portal by Id not found</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Unauthorized</response>
        [Route("ping/{id}")]
        [HttpGet]
        public async Task<ActionResult<PortalResponseViewModel>> PingStickerByIdAsync(Guid id)
        {
            var queryData = await _queryPortalService.QueryByIdAsync(id);
            var portalResponseViewModel = _mapper.Map<PortalResponseViewModel>(queryData.portalResponse);
            portalResponseViewModel.LastFailure = queryData.lastPortalFailureDateTime;
            return Ok(portalResponseViewModel);
        }

        /// <summary>
        /// Pinging portal by full portal
        /// </summary>
        /// <returns>Response</returns>
        /// <response code="200">Response</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("QueryPortal")]
        public async Task<ActionResult<PortalResponse>> QueryPortalAsync([FromBody]PortalQueryPostModel portalPost)
        {
            Portal portal = _mapper.Map<Portal>(portalPost);
            return await _queryPortalService.QueryByPortalAsync(portal);
        }

        /// <summary>
        /// Finds all active stickers from DB and returns
        /// </summary>
        /// <returns></returns>
        /// <response code="200">List of active portals with last error in Db</response>
        /// <response code="204">List of active portals is empty</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Unauthorized</response>
        [Route("GetAllActive")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QueryPortalViewModel>>> GetAllActive()
        {
            var activeListFromDb = await _queryPortalService.GetAllAsync();

            if (activeListFromDb == null)
                return NoContent();

            var activeList = new List<QueryPortalViewModel>();

            foreach (var portal in activeListFromDb)
            {
                activeList.Add(_mapper.Map<QueryPortalViewModel>(portal));
            }

            return Ok(activeList);
        }
        
    }
}