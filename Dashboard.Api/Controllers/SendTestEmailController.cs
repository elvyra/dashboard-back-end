using System;
using System.Threading.Tasks;
using Dashboard.Models;
using Dashboard.Models.Enums;
using Dashboard.Services.EmailNotificationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendTestEmailController : ControllerBase
    {
        private readonly ISendGridService _sendGridService;

        public SendTestEmailController(ISendGridService sendGridService )
        {
            _sendGridService = sendGridService;
        }

        /// <summary>
        /// Sends email to email passes with URL
        /// </summary>
        /// <remarks>
        /// Sends a sample email to entered email address (or default) to test SendGrid 
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="400">No email entered</response>
        [Route("ErrorNotification")]
        [HttpGet]
        public async Task<IActionResult> SendEmailErrorNotification(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("No email entered");

            var portal = new Portal
            {
                Id = Guid.NewGuid(),
                Name = "Test portal",
                Type = PortalType.WebApp,
                URL = "http://test.portal",
                Status = PortalStatus.Active,
                Email = email,
                CheckInterval = 1000,
                Method = RequestMethod.GET,
                BasicAuth = false
            };

            var portalResponse = new PortalResponse
            {
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                Status = 404,
                ResponseTime = 212,
                StatusPageId = portal.Id,
                ErrorMessage = "StatusCode: 404, ReasonPhrase: 'Not Found', Version: 1.1, Content: System.Net.Http.HttpConnectionResponseContent, Headers: { Pragma: no-cache Vary: Accept-Encoding Date: Sun, 08 Mar 2020 02:09:59 GMT Content-Security-Policy: upgrade-insecure-requests Strict-Transport-Security: max-age=63072000; includeSubDomains; preload Cache-Control: max-age=10 Age: 0 Server: DWS Connection: keep-alive Transfer-Encoding: chunked Content-Type: text/html; charset=utf-8 }"
            };

            var response = await _sendGridService.SendEmailAsync(portal, portalResponse);

            return Ok(response);
        }

        /// <summary>
        /// Sends email to email passes with URL
        /// </summary>
        /// <remarks>
        /// Sends a sample email to entered email address (or default) to test SendGrid Permissions changed email
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="400">No email entered</response>
        [Route("PermissionsChanged")]
        [HttpGet]
        public async Task<IActionResult> SendEmailPermissionsChanged(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("No email entered");

            var user = new User
            {
                Name = "Johh",
                Surname = "Smith",
                Email = email,                
                Claims = new string[] { ClaimType.isAdmin.ToString() }
            };

            var response = await _sendGridService.SendPermissionsChangedEmailAsync(email, user);

            return Ok(response);
        }
    }
}