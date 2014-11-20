﻿using System;
using System.Threading.Tasks;
using Exceptionless.Core;
using Exceptionless.Core.Caching;
using Exceptionless.Models;
using Xunit;

namespace Exceptionless.Api.Tests.Caching {
    public class AzureCacheClientTests {
        private readonly AzureCacheClient _cache;

        public AzureCacheClientTests() {
            if (!Settings.Current.UseAzureCache)
                return;

            _cache = new AzureCacheClient(
                endpointUrl: Settings.Current.AzureCacheEndpoint,
                authorizationToken: Settings.Current.AzureCacheAuthorizationToken,
                useLocalCache: false);
        }

        [Fact]
        public void CanSetAndGetValue() {
            if (_cache == null)
                return;

            _cache.Set("test", 1);
            var value = _cache.Get<int>("test");
            Assert.Equal(1, value);
        }

        [Fact]
        public void CanSetExpiration() {
            if (_cache == null)
                return;

            _cache.Set("test", 1, TimeSpan.FromMilliseconds(100));
            var value = _cache.Get<int>("test");
            Assert.Equal(1, value);
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            var newvalue = _cache.Get<int>("test");
            Assert.Equal(0, newvalue);
        }

        [Fact]
        public void CanSetAndGetObject() {
            if (_cache == null)
                return;

            var dt = DateTimeOffset.Now;
            _cache.Set("test", new Event { Type = "test", Date = dt, Message = "Hello World" });
            var value = _cache.Get<Event>("test");

            Assert.Equal(dt, value.Date);
            Assert.Equal("Hello World", value.Message);
            Assert.Equal("test", value.Type);
        }
    }
}
