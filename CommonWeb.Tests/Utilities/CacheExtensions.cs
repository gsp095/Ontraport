using System;
using System.Reflection;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;

namespace HanumanInstitute.CommonWeb.Tests
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Forces the cache to flush, as otherwise it gets shared between tests.
        /// </summary>
        /// <param name="cache">The cache to clear.</param>
        public static void Nuke(this IAppCache cache)
        {
            var cacheProvider = cache.CacheProvider;
            var memoryCache = (MemoryCache)cacheProvider.GetType().GetField("cache", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(cacheProvider)!;
            memoryCache.Compact(1.0);
        }
    }
}
