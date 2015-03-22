﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CacheManager.Core;
using CacheManager.Core.Cache;
using CacheManager.Core.Configuration;
using CacheManager.Memcached;
using CacheManager.StackExchange.Redis;
using CacheManager.SystemRuntimeCaching;

namespace CacheManager.Tests
{
    /// <summary>
    /// Provides some pre configured caches with different setups for testing.
    /// <remarks>
    /// Do not add NullCacheHandle to any of the config. Some tests expect a working implementation...
    /// </remarks>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CacheManagerTestData : IEnumerable<object[]>
    {
        public ICacheManager<object> WithOneMemoryCacheHandleSliding
        {
            get
            {
                return CacheFactory.Build("cache", settings => settings
                    .WithUpdateMode(CacheUpdateMode.Up)
                    .WithHandle<MemoryCacheHandle>("h1")
                        .EnableStatistics()
                        .EnablePerformanceCounters()
                    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10)));
            }
        }

        public ICacheManager<object> WithOneDicCacheHandle
        {
            get
            {
                return CacheFactory.Build("cache", settings => settings
                    .WithUpdateMode(CacheUpdateMode.Full)
                    .WithHandle<DictionaryCacheHandle>("h1")
                        .EnableStatistics()
                    .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10)));
            }
        }

        public ICacheManager<object> WithOneMemoryCacheHandle
        {
            get
            {
                return CacheFactory.Build("cache", settings => settings.WithHandle<MemoryCacheHandle>("h1").EnableStatistics());
            }
        }

        public ICacheManager<object> WithMemoryAndDictionaryHandles
        {
            get
            {
                return CacheFactory.Build("cache", settings =>
                {
                    settings
                        .WithUpdateMode(CacheUpdateMode.None)
                        .WithHandle<MemoryCacheHandle>("h1")
                            .EnableStatistics()
                        .And.WithHandle<MemoryCacheHandle>("h2")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(10))
                        .And.WithHandle<DictionaryCacheHandle>("h3")
                            .EnableStatistics()
                        .And.WithHandle<DictionaryCacheHandle>("h4")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(10));
                });
            }
        }

        public ICacheManager<object> WithManyDictionaryHandles
        {
            get
            {
                return CacheFactory.Build("cache", settings =>
                {
                    settings
                        .WithUpdateMode(CacheUpdateMode.Up)
                        .WithHandle<DictionaryCacheHandle>("h1")
                            .EnableStatistics()
                        .And.WithHandle<DictionaryCacheHandle>("h2")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10))
                        .And.WithHandle<DictionaryCacheHandle>("h3")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(20))
                        .And.WithHandle<DictionaryCacheHandle>("h4")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10))
                        .And.WithHandle<DictionaryCacheHandle>("h5")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(20))
                        .And.WithHandle<DictionaryCacheHandle>("h6")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10))
                        .And.WithHandle<DictionaryCacheHandle>("h7")
                            .EnableStatistics()
                            .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(20));
                });
            }
        }

        public ICacheManager<object> WithTwoNamedMemoryCaches
        {
            get
            {
                return CacheFactory.Build("cache", settings =>
                {
                    settings
                        .WithUpdateMode(CacheUpdateMode.Full)
                        .WithHandle<MemoryCacheHandle>("cache1")
                            .EnableStatistics()
                        .And.WithHandle<MemoryCacheHandle>("cache2")
                            .EnableStatistics();
                });
            }
        }

        public ICacheManager<object> WithRedisCache
        {
            get
            {
                return CacheFactory.Build("cache", settings =>
                {
                    settings
                        .WithMaxRetries(100)
                        .WithRetryTimeout(1000)
                        .WithRedisConfiguration(new RedisConfiguration(
                            "redisCache",
                            new List<ServerEndPoint>() { new ServerEndPoint("127.0.0.1", 6379) },
                            allowAdmin: true
                        //, connectionTimeout: 0 /*<- for testing connection timeout this is handy*/
                            ))
                        .WithHandle<RedisCacheHandle<object>>("redisCache")
                        .EnableStatistics();
                });
            }
        }

        public ICacheManager<object> WithSystemAndRedisCache
        {
            get
            {
                return CacheFactory.Build("cache", settings =>
                {
                    settings
                        .WithUpdateMode(CacheUpdateMode.Up)
                        .WithHandle<MemoryCacheHandle>("cache1")
                            .EnableStatistics();
                    settings
                        .WithMaxRetries(100)
                        .WithRetryTimeout(1000)
                        .WithRedisConfiguration(new RedisConfiguration(
                            "redisCache",
                            new List<ServerEndPoint>() { new ServerEndPoint("127.0.0.1", 6379) },
                            allowAdmin: true
                        //, connectionTimeout: 0 /*<- for testing connection timeout this is handy*/
                            ))
                        .WithHandle<RedisCacheHandle<object>>("redisCache")
                        .EnableStatistics();

                    settings
                        .WithUpdateMode(CacheUpdateMode.Up)
                        .WithHandle<MemoryCacheHandle>("cache2")
                            .EnableStatistics();
                });
            }
        }

        public ICacheManager<object> WithMemcached
        {
            get
            {
                var cache = CacheFactory.Build("myCache", settings =>
                {
                    settings.WithUpdateMode(CacheUpdateMode.Full)
                        .WithHandle<MemcachedCacheHandle<object>>("enyim.com/memcached")
                        .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(100));
                });

                cache.Clear();
                return cache;
            }
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { WithOneMemoryCacheHandleSliding };
            yield return new object[] { WithOneDicCacheHandle };
            yield return new object[] { WithOneMemoryCacheHandle };
            yield return new object[] { WithMemoryAndDictionaryHandles };
            yield return new object[] { WithManyDictionaryHandles };
            yield return new object[] { WithTwoNamedMemoryCaches };
            yield return new object[] { WithRedisCache };
            yield return new object[] { WithSystemAndRedisCache };
            //yield return new object[] { WithMemcached };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}