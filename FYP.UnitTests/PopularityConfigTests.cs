using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using FYP.Main.Trends;
using Xunit;

namespace FYP.UnitTests
{
    public class PopularityConfigTests
    {
        [Fact]
        public void WhenIUseTheLinearConfigWithA2AndAHalfDaySpanItShouldReturnAValueCloseTo50()
        {
            // given
            var span = DateTime.Now - DateTime.Now.AddDays(-2).AddHours(-12);
            var config = new LinearPopularityConfig();

            // when
            var result = config.CalculateScore(span);

            // then
            Assert.True(Math.Abs(50 - result) < 2);
        }

        [Fact]
        public void WhenIUserTheLinearConfigItShouldAlwaysReturnAValueBetween0And100()
        {
            // given
            var config = new LinearPopularityConfig();
            var results = new List<int>();
            var ran = new Random();

            // when
            for (var i = 0; i < 100; ++i)
            {
                results.Add(config.CalculateScore(DateTime.Now - DateTime.Now.AddDays(-ran.Next(0, 250))));
                results.Add(config.CalculateScore(DateTime.Now.AddDays(2) - DateTime.Now.AddDays(ran.Next(-5, 5))));
            }

            // then
            Assert.DoesNotContain(results, x => x < 0 || x > 100);
        }
    }
}
