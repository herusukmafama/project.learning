using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using project.learning.InMemoryCache;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace project.learning.Libs
{
    public class lGSM
    {
        private int timeout = 5;
        private static IConfigurationRoot iConfig, iHdrconfig;
        public IConfigurationRoot execExtAPIPost(string api, string path, string ikey)
        {
            var iCek = InMemoryCacheService._imc.InMemoryCacheChecker(ikey);
            if (iCek == null)
            {
                var WebAPIURL = new dbConn().ConfigGSM(api);
                string requestStr = WebAPIURL + path;
                var serviceProvider = new ServiceCollection().AddHttpClient()
                .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                    options.HttpMessageHandlerBuilderActions.Add(builder =>
                        builder.PrimaryHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                        }))
                .BuildServiceProvider();
                var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
                HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
                client.BaseAddress = new Uri(requestStr);
                client.Timeout = TimeSpan.FromMinutes(timeout);
                var contentData = new StringContent(JsonConvert.SerializeObject(new { SecretId = ikey }), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
                string strjson = response.Content.ReadAsStringAsync().Result;
                client.Dispose();

                JObject ijob = JObject.Parse(strjson);
                if (Convert.ToBoolean(ijob["isSuccess"]))
                {
                    List<dynamic> ilist = new List<dynamic>();
                    ilist.Add(new { secret = ijob["secret"].ToString() });
                    InMemoryCacheService._imc.AddInMemoryCache(ikey, ilist, CacheItemPriority.High);
                }
                var ireturn = InMemoryCacheService._imc.InMemoryCacheChecker(ikey);

                byte[] byteArray = Encoding.UTF8.GetBytes(ireturn[0].secret);
                MemoryStream stream = new MemoryStream(byteArray);

                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonStream(stream);

                if (ikey == "master_appsettings")
                {
                    return iConfig = builder.Build();
                }
                else
                {
                    return iHdrconfig = builder.Build();
                }
            }
            else
            {
                if (ikey == "master_appsettings")
                {
                    return iConfig;
                }
                else
                {
                    return iHdrconfig;
                }
            }
        }

    }
}
