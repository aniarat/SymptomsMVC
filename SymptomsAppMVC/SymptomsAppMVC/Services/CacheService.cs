using Microsoft.Extensions.Caching.Memory;

namespace SymptomsAppMVC.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getData, TimeSpan cacheDuration)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out T cacheEntry))
            {
                cacheEntry = await getData();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(cacheDuration);

                _memoryCache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}
