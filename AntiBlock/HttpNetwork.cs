using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProxyNetwork;

namespace AntiBlock
{
    public class HttpNetwork
    {
        private readonly IProxyProvider _proxyProvider;
        private readonly string[] _badContentPatterns;
        private readonly string[] _blokedPagePatterns;
        private readonly HttpNetworkSettings _settings;

        public HttpNetwork(HttpNetworkSettings settings)
        {
            _settings = settings;
            _proxyProvider = settings.ProxyProvider;
            _badContentPatterns = settings.BadContentPatterns;
            _blokedPagePatterns = settings.BlokedPagePatterns;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, params string[] contentPatterns)
        {
            using (var client = await CreateClient())
            {
                try
                {
                    var source = new CancellationTokenSource(_settings.Timeout);
                    var response = await client.GetAsync(url, source.Token);
                    var content = await response.Content.ReadAsStringAsync();
                    if (IsBlocked(content))
                    {
                        throw new BlockingException(client.BaseAddress.Host, client.Identity?.Proxy.Host ?? "LocalIPAddress");
                    }
                    if (IsBadProxy(content, contentPatterns))
                    {
                        throw new ContentException("The response content isn't correct (bad proxy)", url, content);
                    }
                    return response;
                }
                catch (Exception e)
                {
                    if (_proxyProvider != null)
                    {
                        _proxyProvider.Remove(client.Identity, e is BlockingException || e is ContentException);
                        return await GetAsync(url, contentPatterns);
                    }
                    throw;
                }
            }
        }

        private async Task<HttpClientPro> CreateClient()
        {
            var clientPro = _proxyProvider != null 
                ? new HttpClientPro(await _proxyProvider.NextAsync()) 
                : new HttpClientPro();
            clientPro.BaseAddress = new Uri(_settings.Host);
            clientPro.DefaultRequestHeaders.Accept.Clear();
            clientPro.DefaultRequestHeaders.Accept.ParseAdd(_settings.Accept);
            clientPro.DefaultRequestHeaders.AcceptLanguage.Clear();
            clientPro.DefaultRequestHeaders.AcceptLanguage.ParseAdd(_settings.AcceptLanguage);
            return clientPro;
        }

        private bool IsBlocked(string content)
        {
            return _blokedPagePatterns.Any(pattern => Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
        }

        private bool IsBadProxy(string content, IEnumerable<string> contentPatterns)
        {
            return _badContentPatterns.Any(pattern => Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase)) 
                || contentPatterns.Any(pattern => !Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
        }
    }
}