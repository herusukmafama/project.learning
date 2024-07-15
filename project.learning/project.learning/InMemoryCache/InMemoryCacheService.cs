using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace project.learning.InMemoryCache
{
    public class InMemoryCacheService : IInMemoryCache
    {
        private readonly IMemoryCache _memoryCache;

        public static IInMemoryCache _imc { get; set; }
        private int absoluteExpirationValue { get; set; }
        private int slidingExpirationValue { get; set; }

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            #region Json-Config
            var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            absoluteExpirationValue = configuration.GetValue<int>("Secret:InMemoryCacheConfigurations:AbsoluteExpiration");
            slidingExpirationValue = configuration.GetValue<int>("Secret:InMemoryCacheConfigurations:SlidingExpiration");
            #endregion
        }

        public void AddInMemoryCache(string key, List<dynamic> list, CacheItemPriority priority)
        {
            //setting up cache options
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(absoluteExpirationValue),  //by-appsettings
                Priority = priority,                                                    //type of priority (Low, Normal, High, NeverRemove)
                SlidingExpiration = TimeSpan.FromSeconds(slidingExpirationValue)        //by-appsettings        
            };
            //setting cache entries
            _memoryCache.Set(key, list, cacheExpiryOptions);
        }

        public List<dynamic> InMemoryCacheChecker(string key)
        {
            //checks if cache entries exists
            bool isExist = _memoryCache.TryGetValue(key, out List<dynamic> dynamicList);
            
            return dynamicList;
        }

        public void RemoveInMemoryCache(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
