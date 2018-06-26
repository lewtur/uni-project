using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FYP.Data;
using FYP.Models;
using FYP.Models.Abstractions;

namespace FYP.External.DataSources
{
    public class ArtistDescriptionDataSource : IDataSource
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IArtistInfoRetriever _artistInfoRetriever;

        public ArtistDescriptionDataSource(IArtistRepository artistRepository, IArtistInfoRetriever artistInfoRetriever)
        {
            _artistRepository = artistRepository;
            _artistInfoRetriever = artistInfoRetriever;
        }

        public string GetName()
        {
            return "Last.fm artist description";
        }

        public void Update(INamedEntity source)
        {
            var artist = source as Artist;

            if (artist == null || artist.Id == 0) return;
            if (!string.IsNullOrEmpty(artist.Description)) return;           

            var bio = _artistInfoRetriever.GetArtistDescription(source.Name);
            if (string.IsNullOrEmpty(bio)) return;

            var escaped = System.Security.SecurityElement.Escape(bio);

            _artistRepository.UpdateArtistDescription(escaped, artist.Id);                        
        }
    }
}
