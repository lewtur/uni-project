using System;
using System.Collections.Generic;
using System.Text;
using FYP.Models.Abstractions;

namespace FYP.Models
{
    public class Venue : INamedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public SqlVenue GetSqlFriendlyVenue()
        {
            return new SqlVenue
            {
                Name = Name,
                Address = Address,
                Town = Town,
                PostCode = PostCode,
                Latitude = Latitude,
                Longitude = Longitude
            };
        }

        public class SqlVenue
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Town { get; set; }
            public string PostCode { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }

    }    
}
