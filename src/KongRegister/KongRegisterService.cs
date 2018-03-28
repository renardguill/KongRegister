using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KongRegister.Extensions;

namespace KongRegister
{
    public class KongRegisterService : ABackgroundService
    {
        private readonly KongRegisterConfig _kongConfig;
        private readonly string _kongUrl;
        private string _targetHost;
        private int _targetPort;
        private string _targetId;

        private readonly ILogger<KongRegisterService> _logger;
        private readonly IServer _server;

        /// <summary>
        /// Service for registering the hosted application in Kong API Gateway.
        /// </summary>
        /// <param name="kongConfig"></param>
        /// <param name="logger"></param>
        /// <param name="server"></param>
        public KongRegisterService(IOptions<KongRegisterConfig> kongConfig, ILogger<KongRegisterService> logger, IServer server)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _kongConfig = kongConfig.Value ?? throw new ArgumentNullException(nameof(kongConfig));
            try
            {
                _kongConfig.Validate();
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error configuration file {ex.Message}");
            }

            _kongUrl = $"{_kongConfig.KongApiUrl}/upstreams/{_kongConfig.UpstreamId}/targets";

        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"KongRegisterService is starting.");

            stoppingToken.Register(() =>
                    _logger.LogInformation($"KongRegister background task is stopping."));


            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"KongRegister background task is doing background work.");
                _targetId = await RegisterAsync();
            }
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await UnregisterAsync(_targetId);
        }


        private async Task<string> RegisterAsync()
        {

            _logger.LogInformation("Registering target in Kong");

            if (_kongConfig.TargetHostDiscovery != null 
                && _kongConfig.TargetHostDiscovery.Equals("dynamic", StringComparison.InvariantCultureIgnoreCase))
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("1.2.3.4", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    _targetHost = endPoint.Address.ToString();
                }
            }
            else
            {
                _targetHost = _kongConfig.TargetHost;
            }

            if (_kongConfig.TargetPortDiscovery != null 
                && _kongConfig.TargetPortDiscovery.Equals("dynamic", StringComparison.InvariantCultureIgnoreCase))
            {
                var features = _server.Features;
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First();
                _targetPort = new Uri(address).Port;
            }
            else
            {
                _targetPort = (int)_kongConfig.TargetPort;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(_kongConfig.KongApiKeyHeader, _kongConfig.KongApiKey);
                    var response = await client.PostAsJsonAsync(_kongUrl, new
                    {
                        target = string.Join(":", _targetHost, _targetPort.ToString()),
                        weight = _kongConfig.TargetWeight
                    });
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        var created = await response.Content.ReadAsAsync<dynamic>();
                        _logger.LogInformation($"Target {created.id} registered in Kong.");
                        return created.id;
                    }
                    else
                    {
                        _logger.LogError($"Failed to register target in Kong : {(int)response.StatusCode} {response.ReasonPhrase}");
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Registeration failed");
                throw;
            }
        }
        private async Task UnregisterAsync(string targetId)
        {
            _logger.LogInformation($"Unregistering target {targetId} from Kong");
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(_kongConfig.KongApiKeyHeader, _kongConfig.KongApiKey);
                    var response = await client.DeleteAsync(_kongUrl + "/" + targetId);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        _logger.LogInformation($"Target {targetId} ungregistred.");
                    }
                    else
                    {
                        _logger.LogError($"Error to unregistrering traget {targetId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unregisteration failed");
            }
        }
    }
}
