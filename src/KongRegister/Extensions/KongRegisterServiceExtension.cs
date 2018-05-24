using System;
using KongRegister.Business;
using KongRegister.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KongRegister.Extensions
{
    public static class KongRegisterServiceExtension
    {
        /// <summary>
        /// KongRegisterServiceExtension
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">Configuration</param>
        /// <param name="sectionName">KongRegister section configuration name</param>
        /// <returns>Services collection</returns>
        public static IServiceCollection ConfigureKongRegister(this IServiceCollection services, IConfiguration configuration, string sectionName = "KongRegister")
        {
            var kongRegisterConfig = configuration.GetSection(sectionName);
            services.Configure<KongRegisterConfig>(kongRegisterConfig);
            
            var krc = kongRegisterConfig.Get<KongRegisterConfig>();

            // SI pas de config par défaut
            if (kongRegisterConfig[nameof(krc.Disabled)] == null) kongRegisterConfig[nameof(krc.Disabled)] = krc.Disabled.ToString();
            if (kongRegisterConfig[nameof(krc.OnStartup)] == null) kongRegisterConfig[nameof(krc.OnStartup)] = krc.OnStartup.ToString();


            // Variables d'environnement si elles existent
            if (bool.TryParse(configuration.GetValue<string>("KR_DISABLED"), out bool disabled))
            {
                kongRegisterConfig[nameof(krc.Disabled)] = disabled.ToString();
            }
            if (bool.TryParse(configuration.GetValue<string>("KR_ONSTARTUP"), out bool onStratup))
            {
                kongRegisterConfig[nameof(krc.OnStartup)] = onStratup.ToString();
            }

            // Register
            if (!Convert.ToBoolean(kongRegisterConfig[nameof(krc.Disabled)]))
            {
                services.AddSingleton<IKongRegisterBusiness, KongRegisterBusiness>();
                if (Convert.ToBoolean(kongRegisterConfig[nameof(krc.OnStartup)]))
                {
                    services.AddSingleton<IHostedService, KongRegisterBackgroudService>();
                }
            }
            return services;
        }
    }
}
