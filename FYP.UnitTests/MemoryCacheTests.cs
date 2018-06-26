using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FYP.Main;
using Xunit;

namespace FYP.UnitTests
{
    public class MemoryCacheTests
    {
        [Fact]
        public void ShouldBeAbleAddAnItemToTheCacheThenReadIt()
        {
            // given
            const string toAdd = "potato";

            // when
            MemoryCache.Instance.Add(toAdd, "KEY1", DateTime.Now.AddDays(2));
            var returned = MemoryCache.Instance.Get("KEY1");

            // then
            Assert.Equal(toAdd, returned);
        }

        [Fact]
        public void WhenAnItemExpiresFromTheCacheAndITryGetItItShouldReturnNull()
        {
            // given
            const string toAdd = "egg";

            // when
            MemoryCache.Instance.Add(toAdd, "KEY2", DateTime.Now);
            Thread.Sleep(30);
            var returned = MemoryCache.Instance.Get("KEY2");

            // then
            Assert.NotEqual(toAdd, returned);
            Assert.Null(returned);
        }

        [Fact]
        public void WhenIRemoveAnItemFromTheCacheAndTryAndReadItItShouldReturnNull()
        {
            // given
            const string toAdd = "cucumber";

            // when
            MemoryCache.Instance.Add(toAdd, "KEY3", DateTime.Now.AddDays(3));
            MemoryCache.Instance.Remove("KEY3");
            var returned = MemoryCache.Instance.Get("KEY3");

            // then
            Assert.NotEqual(toAdd, returned);
            Assert.Null(returned);
        }

        [Fact]
        public void WhenIAddAnItemToTheCacheWhosKeyAlreadyExistsItShouldReplaceTheOldValue()
        {
            // given
            const string first = "aubergine";
            const string second = "carrot";

            // when
            MemoryCache.Instance.Add(first, "KEY4", DateTime.Now.AddDays(4));
            MemoryCache.Instance.Add(second, "KEY4", DateTime.Now.AddDays(4));
            var returned = MemoryCache.Instance.Get("KEY4");

            // then
            Assert.Equal(returned, second);
        }

        [Fact]
        public void WhenIGetAnItemThatWasNeverInTheCacheItShouldReturnNull()
        {
            // when
            var returned = MemoryCache.Instance.Get("who lives in a pineapple under the sea");

            // then
            Assert.Null(returned);
        }
    }
}
