using Asp.Versioning;
using CleanArc.Application.Features.PermissionManager.Command;
using CleanArc.Application.Features.PermissionManager.Query;
using CleanArc.Application.Models.Dto.Admin.Controller;
using CleanArc.WebFramework.BaseController;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArc.Web.Api.Controllers.Areas.Admin.PermissionManager
{
    [Authorize(Roles = "admin")]
    [Area("admin")]
    [ApiVersion("1")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[area]/PermissionManager")]
    public class PermissionManagerController(ISender sender) : BaseController
    {
        /// <summary>
        /// دریافت اندپوینت ها
        /// </summary>
        /// <returns></returns> 
        [HttpGet("GetControllers")]
        public async Task<List<EndpointsResponseDto>?> GetControllersAync()
        {
            var response = new GetControllersQuery("admin");

            return await sender.Send(response);

        }

        /// <summary>
        /// تعریف دسترسی
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("SetPermissions")]
        public async Task<IActionResult> SetPermissionForEndpoints([FromBody]SetPermissionForControllerCommand Command) 
        {
            await sender.Send(Command);

            return Ok("دسترسی با موفقیت اضافه شد");
        }

        /// <summary>
        /// دریافت دسترسی ها
        /// </summary>
        /// <param name="Area"></param>
        /// <returns></returns>
        [HttpGet("GetPermissions")]
        public async Task<List<GetPermissionsResponseDto>> GetPermissions(string? Area) 
        {
            var query = new GetPermissionsQuery(Area);

            return await sender.Send(query);

        }

        /// <summary>
        /// ایجاد دسترسی برای کاربران
        /// </summary>
        /// <returns></returns>
        [HttpPost("SetAccessForUsers")]
        public async Task<IActionResult> SetAccess(SetAccessForUsersCommand command) 
        {
            await sender.Send(command);

            return Ok("دسترسی با موفقیت به کاربر تخصیص شد");
        }
    }
}
