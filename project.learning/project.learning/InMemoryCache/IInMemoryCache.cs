using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project.learning.InMemoryCache
{
    public interface IInMemoryCache
    {
        public List<dynamic> InMemoryCacheChecker(string key);
        public void AddInMemoryCache(string key, List<dynamic> list, CacheItemPriority priority);
        public void RemoveInMemoryCache(string key);
    }
}
