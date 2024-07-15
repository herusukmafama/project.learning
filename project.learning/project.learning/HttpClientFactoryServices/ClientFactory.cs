using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace project.learning.HttpClientFactoryServices
{
    public class ClientFactory : IClientFactory
    {
        private IHttpClientFactory _clientFactory;
        //private static HttpClient _httpClient;
        //private static HttpClient _httpClientToken;

        public ClientFactory(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

            //if (_httpClient == null)
            //{
            //    _httpClient = clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            //}

            //if (_httpClientToken == null)
            //{
            //    _httpClientToken = clientFactory.CreateClient("HttpClientTokenWithSSLUntrusted");
            //}            
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var _httpClient = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            _httpClient.DefaultRequestHeaders.ConnectionClose = true;
            //_httpClient.BaseAddress = new Uri(url);

            HttpResponseMessage result = await _httpClient.GetAsync(url);

            return result;
        }

        public async Task<HttpResponseMessage> GetAsyncWithToken(string url, string credential)
        {
            var _httpClientToken = _clientFactory.CreateClient("HttpClientTokenWithSSLUntrusted");
            _httpClientToken.DefaultRequestHeaders.ConnectionClose = true;
            //_httpClientToken.BaseAddress = new Uri(url);

            if (!_httpClientToken.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                _httpClientToken.DefaultRequestHeaders.Remove("Authorization");
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage result = await _httpClientToken.GetAsync(url);

            return result;
        }

        public async Task<HttpResponseMessage> GetAsyncWithTokenCancellation(string url, string credential, CancellationToken cancellation)
        {
            var _httpClientToken = _clientFactory.CreateClient("HttpClientTokenWithSSLUntrusted");
            _httpClientToken.DefaultRequestHeaders.ConnectionClose = true;
            //_httpClientToken.BaseAddress = new Uri(url);

            if (!_httpClientToken.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                _httpClientToken.DefaultRequestHeaders.Remove("Authorization");
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage result = await _httpClientToken.GetAsync(url, cancellation);

            return result;
        }

        public Task<HttpResponseMessage> GetAsyncWithCustomRequestHeader(string url, JObject json)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> PostAsync(string url, StringContent content)
        {
            var _httpClient = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");
            //_httpClient.BaseAddress = new Uri(url);

            HttpResponseMessage result = await _httpClient.PostAsync(url, content);

            return result;
        }

        public async Task<HttpResponseMessage> PostAsyncWithToken(string url, StringContent content, string credential)
        {
            var _httpClientToken = _clientFactory.CreateClient("HttpClientTokenWithSSLUntrusted");
            //_httpClientToken.BaseAddress = new Uri(url);

            if (!_httpClientToken.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                _httpClientToken.DefaultRequestHeaders.Remove("Authorization");
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage result = await _httpClientToken.PostAsync(url, content);

            return result;
        }

        public async Task<HttpResponseMessage> PostAsyncWithTokenCancellation(string url, StringContent content, string credential, CancellationToken cancellation)
        {
            var _httpClientToken = _clientFactory.CreateClient("HttpClientTokenWithSSLUntrusted");
            //_httpClientToken.BaseAddress = new Uri(url);

            if (!_httpClientToken.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                _httpClientToken.DefaultRequestHeaders.Remove("Authorization");
                _httpClientToken.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage result = await _httpClientToken.PostAsync(url, content, cancellation);

            return result;
        }

        public async Task<HttpResponseMessage> PostAsyncWithCustomRequestHeader(string url, StringContent content, JObject json)
        {
            var _httpClient = _clientFactory.CreateClient("HttpClientWithSSLUntrusted");

            if (json != null)
            {
                foreach (var item in json)
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value.ToString());
                }
            }

            HttpResponseMessage result = await _httpClient.PostAsync(url, content);

            return result;
        }


        //public HttpClient CallAsync(string clientName)
        //{
        //    HttpClient client = _clientFactory.CreateClient(clientName);
        //    return client;
        //}
    }
}