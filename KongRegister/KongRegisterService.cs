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

namespace KongRegister
{
    public class KongRegisterService : ABackgroundService
    {
        private readonly KongConfig _kongConfig;
        private readonly string _kongUrl;
        private string _localIP;
        private string _localPort;
        private string _targetId;

        private readonly ILogger<KongRegisterService> _logger;
        private readonly IServer _server;

        /// <summary>
        /// Service for registering the hosted application in Kong API Gateway.
        /// </summary>
        /// <param name="kongConfig"></param>
        /// <param name="logger"></param>
        /// <param name="server"></param>
        public KongRegisterService(IOptions<KongConfig> kongConfig, ILogger<KongRegisterService> logger, IServer server)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _kongConfig = kongConfig.Value ?? throw new ArgumentNullException(nameof(kongConfig));

            _kongUrl =
                _kongConfig.BaseUrl +
                _kongConfig.UpstreamsUri +
                _kongConfig.UpstreamNameUri +
                _kongConfig.TargetsUri;

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
            var features = _server.Features;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("1.2.3.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                _localIP = endPoint.Address.ToString();
            }
            _localPort = new Uri(address).Port.ToString();


            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add(_kongConfig.KongApiKeyHeader, _kongConfig.KongApiKey);
                    var response = await client.PostAsJsonAsync(_kongUrl, new
                    {
                        target = string.Join(":", _localIP, _localPort),
                        weight = _kongConfig.TargetWeight
                    });
                    var created = await response.Content.ReadAsAsync<dynamic>();

                    _logger.LogInformation($"Target {created.id} registered in Kong.");
                    return created.id;
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
