using System;
using FYP.Models.Abstractions;

namespace FYP.Models
{
    public class Artist : INamedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpotifyRecordId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
