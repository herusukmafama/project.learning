using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using project.learning.InMemoryCache;
using project.learning.HttpClientFactoryServices;
using System.Net.Http.Headers;
using System.Net.Http;
using project.learning.Libs;
using project.learning.Model;
using Newtonsoft.Json.Linq;

namespace project.learning
{
    public partial class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllers();
            services.AddMetrics();
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.Configure<KestrelServerOptions>(Configuration.GetSection("Kestrel"));
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            #region httpClientFactory

            services.AddHttpClient("HttpClientWithSSLUntrusted", c =>
            {
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.Timeout = TimeSpan.FromMinutes(5);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
            });

            services.AddHttpClient("HttpClientTokenWithSSLUntrusted", c =>
            {
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.Timeout = TimeSpan.FromMinutes(5);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
            });

            #endregion

            #region Client-Scoped
            services.AddScoped<IClientFactory, ClientFactory>();
            #endregion

            #region MemoryCache
            services.AddMemoryCache();
            services.AddScoped<IInMemoryCache, InMemoryCacheService>();
            #endregion MemoryCache
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInMemoryCache imc)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region MemoryCache
            InMemoryCacheService._imc = imc;
            #endregion MemoryCache

            app.UseCors("MyPolicy");
            ConfigureAuth(app);
            app.UseHttpsRedirection();
            app.UseMvc();

            #region EPV OCBC

            IHttpClientFactory _clientFactory = app.ApplicationServices.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;

            string key1 = Configuration.GetSection("EPV:keyIvUrl1").Value;
            string key2 = Configuration.GetSection("EPV:keyIvUrl2").Value;

            List<string> keyUrl = new List<string> { key1, key2 };

            for (int i = 0; i < keyUrl.Count; i++)
            {
                string[] key = keyUrl[i].Split(";");

                var result = this.execExtAPIGet(_clientFactory, key[2]);
                JObject dataResp = JObject.Parse(result);

                if (dataResp.GetValue("responseCode").ToString() == "200" && dataResp.GetValue("responseMessage").ToString() == "success")
                {
                    if (i == 0)
                    {
                        EpvCredentialModel.username_1 = dataResp["data"]["strUserName"].ToString();
                        EpvCredentialModel.password_1 = lEPVDecyrpt.DecryptStringFromBase64String_Aes(System.Text.Encoding.UTF8.GetBytes(key[0]), System.Text.Encoding.UTF8.GetBytes(key[1]), dataResp["data"]["strPassword"].ToString());
                        EpvCredentialModel.host_1 = dataResp["data"]["strServerIp"].ToString();
                        EpvCredentialModel.port_1 = dataResp["data"]["strServerPort"].ToString();
                    }
                    else if (i == 1)
                    {
                        EpvCredentialModel.username_2 = dataResp["data"]["strUserName"].ToString();
                        EpvCredentialModel.password_2 = lEPVDecyrpt.DecryptStringFromBase64String_Aes(System.Text.Encoding.UTF8.GetBytes(key[0]), System.Text.Encoding.UTF8.GetBytes(key[1]), dataResp["data"]["strPassword"].ToString());
                        EpvCredentialModel.host_2 = dataResp["data"]["strServerIp"].ToString();
                        EpvCredentialModel.port_2 = dataResp["data"]["strServerPort"].ToString();
                    }
                }
            }
        }

        public string execExtAPIGet(IHttpClientFactory httpClientFactory, string url)
        {
            string result = "";
            result = execGetApi(httpClientFactory, url).Result;
            return result;
        }

        public async Task<string> execGetApi(IHttpClientFactory httpClientFactory, string url)
        {
            var _client = httpClientFactory.CreateClient();

            JObject epvKey = new JObject();
            epvKey.Add(new JProperty("id", new JValue(url)));

            using HttpResponseMessage response = await _client.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            return result;
        }
        #endregion
    }

}
