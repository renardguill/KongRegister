using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using KongRegister.Business.Interfaces;

namespace KongRegister
{
    public class KongRegisterBackgroudService : ABackgroundService
    {
        private readonly ILogger<KongRegisterBackgroudService> _logger;
        private readonly IKongRegisterBusiness _kongRegisterBusiness;

        /// <summary>
        /// Backgroud service for registering the hosted application in Kong API Gateway.
        /// </summary>
        /// <param name="kongRegisterBusiness"></param>
        /// <param name="logger"></param>
        public KongRegisterBackgroudService(IKongRegisterBusiness kongRegisterBusiness, ILogger<KongRegisterBackgroudService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _kongRegisterBusiness = kongRegisterBusiness ?? throw new ArgumentNullException(nameof(kongRegisterBusiness));
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_kongRegisterBusiness.OnStartup())
            {
                _logger.LogInformation($"KongRegisterBakgroudService is starting.");

                stoppingToken.Register(() =>
                        _logger.LogInformation($"KongRegister background task is stopping."));

                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"KongRegister background task is doing background work.");
                    await _kongRegisterBusiness.RegisterAsync();
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_kongRegisterBusiness.OnStartup())
            {
                await _kongRegisterBusiness.UnregisterAsync();
            }
        }
    }
}
