using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace project.learning.HttpClientFactoryServices
{
    public interface IClientFactory
    {
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> GetAsyncWithToken(string url, string credential);
        Task<HttpResponseMessage> GetAsyncWithTokenCancellation(string url, string credential, CancellationToken cancellation);
        Task<HttpResponseMessage> GetAsyncWithCustomRequestHeader(string url, JObject json);
        Task<HttpResponseMessage> PostAsync(string url, StringContent content);
        Task<HttpResponseMessage> PostAsyncWithToken(string url, StringContent content, string credential);
        Task<HttpResponseMessage> PostAsyncWithTokenCancellation(string url, StringContent content, string credential, CancellationToken cancellation);
        Task<HttpResponseMessage> PostAsyncWithCustomRequestHeader(string url, StringContent content, JObject json);
    }
}
