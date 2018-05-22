using KongRegister;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KongRegisterConfig>(Configuration.GetSection("KongRegister"));
            services.PostConfigure<KongRegisterConfig>(kongConf =>
            {
                if (bool.TryParse(Configuration.GetValue<string>("KR_DISABLED"), out bool disabled))
                {
                    kongConf.Disabled = disabled;
                }

                if (bool.TryParse(Configuration.GetValue<string>("KR_ONSTARTUP"), out bool onStratup))
                {
                    kongConf.OnStartup = onStratup;
                }

            });
            services.AddSingleton<IHostedService, KongRegisterService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
