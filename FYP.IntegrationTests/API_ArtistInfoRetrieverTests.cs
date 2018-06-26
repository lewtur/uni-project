using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using FYP.Data;
using FYP.External;
using FYP.External.DataSources;
using FYP.Models;
using Moq;
using Xunit;

namespace FYP.IntegrationTests
{
    public class API_ArtistInfoRetrieverTests
    { 
        [Fact]
        public void ShouldBeAbleToUpdateTheDescriptionOfAnArtistUsingTheLastFmDataSource()
        {
            // given
            var infoRetriever = new ArtistInfoRetriever(new HttpRequester());

            // when
            var result = infoRetriever.GetArtistDescription("Blossoms");

            // then
            Assert.False(string.IsNullOrEmpty(result));
        }
    }
}
