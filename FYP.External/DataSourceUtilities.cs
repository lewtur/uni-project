using System;
using System.Collections.Generic;
using System.Text;
using FYP.Models.Abstractions;

namespace FYP.External
{
    public class DataSourceUtilities
    {
        private readonly ICache _cache;
        private readonly IHttpRequester _httpRequester;

        public DataSourceUtilities(ICache cache, IHttpRequester httpRequester)
        {
            _cache = cache;
            _httpRequester = httpRequester;
        }
    }
}
