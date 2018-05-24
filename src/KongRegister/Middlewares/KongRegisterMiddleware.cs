using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KongRegister.Business.Interfaces;
using Microsoft.AspNetCore.Http;

namespace KongRegister.Middlewares
{
    public class KongRegisterMiddleware
    {
        private RequestDelegate _next;
        private readonly string _registerRoute;
        private readonly string _unregisterRoute;

        public KongRegisterMiddleware(RequestDelegate next, string registerRoute, string unregisterRoute)
        {
            _next = next;
            _registerRoute = registerRoute;
            _unregisterRoute = unregisterRoute;
        }

        public async Task InvokeAsync(HttpContext context, IKongRegisterBusiness kongRegisterBusiness)
        {
            var _kongRegisterBusiness = kongRegisterBusiness;
            var path = context.Request.Path;
            if (path == _registerRoute)
            {
                var targetId = await kongRegisterBusiness.RegisterAsync();
                await context.Response.WriteAsync($"Target {targetId} registered in Kong.");
            }
            else if (path == _unregisterRoute)
            {
                var unregisterd = await kongRegisterBusiness.UnregisterAsync();
                if (unregisterd)
                {
                    await context.Response.WriteAsync($"Target unregistered from Kong.");
                }
                else
                {
                    await context.Response.WriteAsync($"Unable to unregister target from Kong.");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
