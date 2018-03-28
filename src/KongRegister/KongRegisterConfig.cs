namespace KongRegister
{
    public class KongRegisterConfig
    {
   
        public string KongApiUrl { get; set; }
        public string KongApiKeyHeader { get; set; }
        public string KongApiKey { get; set; }
        public string UpstreamId { get; set; }
        public string TargetHostDiscovery { get; set; }
        public string TargetHost { get; set; }
        public string TargetPortDiscovery { get; set; }
        public int? TargetPort { get; set; }
        public int? TargetWeight { get; set; }
    }
}