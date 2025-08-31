using CleanArc.Application.Contracts.Persistence;
using CleanArc.Application.Models.Dto.Admin.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleanArc.Web.Api.Controllers
{
    public class EndpointReflector : IEndpointReflector
    {
        private readonly Assembly _assembly;

        public EndpointReflector()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }
        public List<GetControllersResponseDto> GetEndpointsByArea(string Area)
        {
            var endpoints = new List<GetControllersResponseDto>();

            var controllers = _assembly.GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type)
                && !type.IsAbstract && type.GetCustomAttribute<AreaAttribute>()?.RouteValue == $"{Area}");

            foreach (var controller in controllers)
            {
                foreach (var method in controller.GetMethods())
                {
                    var hasHttpAttribute = method
                    .GetCustomAttributes(typeof(HttpMethodAttribute), inherit: true)
                    .Any();

                    if (!hasHttpAttribute)
                        continue;

                    endpoints.Add(new GetControllersResponseDto
                    {
                        ControllerName = controller.Name,
                        ActionName = method.Name,
                        Area = Area,
                    });
                }
            }

            return endpoints;
        }
    }
}
