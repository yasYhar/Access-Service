using CleanArc.Application.Contracts.Persistence;
using CleanArc.SharedKernel.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArc.WebFramework.Filters
{
    public class ApiAccessActionFilter(IUnitOfWork unitOfWork, IConfiguration
        configuration) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isFeatureActiveValue = configuration.GetSection("Features:PermissionBaseFeature").Value;
            if (!string.IsNullOrEmpty(isFeatureActiveValue) && isFeatureActiveValue.ToLower() == "true")
            {
                var userIdString = context.HttpContext.User.Identity?.GetUserId();
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    context.Result = new JsonResult(new { message = "کاربر احراز هویت نشده است" })
                    {
                        StatusCode = 401
                    };
                    return;
                }

                var routeEndPoint = context.RouteData.Values["action"]?.ToString();
                var routeControllerName = context.RouteData.Values["controller"]?.ToString();
                var routeControllerArea = context.RouteData.Values["area"]?.ToString() ?? "";

                if (string.IsNullOrEmpty(routeEndPoint) || string.IsNullOrEmpty(routeControllerName))
                {
                    context.Result = new JsonResult(new { message = "مسیر نامعتبر است" })
                    {
                        StatusCode = 400
                    };
                    return;
                }


                string routeData = $"{routeControllerArea}/{routeControllerName}Controller/{routeEndPoint}";

                if (routeControllerArea.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    bool haveAccess = false;

                    try
                    {
                        var permissions = await unitOfWork.UserPermissionsRepository.GetPermissionsWithUserId(userId);

                        if (permissions != null && permissions.Any())
                        {
                            foreach (var permission in permissions)
                            {
                                if (permission?.Permissions == null) continue;

                                string area = permission.Permissions.Area ?? "";
                                string controllerName = permission.Permissions.ControllerName ?? "";
                                string endPoint = permission.Permissions.EndPoint ?? "";
                                string route = $"{area}/{controllerName}/{endPoint}";

                                if (routeData.Equals(route, StringComparison.OrdinalIgnoreCase))
                                {
                                    haveAccess = true;
                                    break;
                                }
                            }
                        }

                        if (!haveAccess)
                        {
                            context.Result = new JsonResult(new { message = "شما به این بخش دسترسی ندارید" })
                            {
                                StatusCode = 403
                            };
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Not Access : {userId}");
                        context.Result = new JsonResult(new { message = "خطا در بررسی دسترسی" })
                        {
                            StatusCode = 500
                        };
                        return;
                    }
                }

                await next();
                
            }

            await next();
            return;
        }
    }
}
