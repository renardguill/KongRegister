namespace KongRegister
{
    public class KongConfig
    {
        public string KongApiKeyHeader { get; set; }
        public string KongApiKey { get; set; }
        public string BaseUrl { get; set; }
        public string UpstreamsUri { get; set; }
        public string UpstreamNameUri { get; set; }
        public string TargetsUri { get; set; }
        public int TargetWeight { get; set; }
    }
}