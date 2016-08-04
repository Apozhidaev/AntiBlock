using ProxyNetwork;

namespace AntiBlock
{
    public class HttpNetworkSettings
    {
        public HttpNetworkSettings()
        {
            Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            AcceptLanguage = "en-US;q=0.7,en;q=0.3";
            Timeout = 30*1000; // 30 sec
            BadContentPatterns = new string[0];
            BlokedPagePatterns = new string[0];
        }

        public string Host { get; set; }
        public string Accept { get; set; }
        public string AcceptLanguage { get; set; }
        public int Timeout { get; set; }
        public int Interval { get; set; }
        public IProxyProvider ProxyProvider { get; set; }
        public string[] BadContentPatterns { get; set; }
        public string[] BlokedPagePatterns { get; set; }
    }
}