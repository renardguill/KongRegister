using KongRegister.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace KongRegister.Extensions
{
    public static class KongRegisterMiddlewareExtension
    {
        public static IApplicationBuilder UseKongRegisterController(this IApplicationBuilder app, string registerRoute = "/kongregister", string unregisterRoute = "/kongunregister")
        {
            return app.UseMiddleware<KongRegisterMiddleware>(registerRoute, unregisterRoute);
        }
    }
}
