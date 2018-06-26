using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Data.DataSourceRepositories;
using FYP.Models.Abstractions;

namespace FYP.Main.PostUpdateActions
{
    public class AssignStatsToArtists : IPostUpdateAction
    {
        private readonly ITwitterDataSourceRepository _twitterRepository;

        public AssignStatsToArtists(ITwitterDataSourceRepository twitterRepository)
        {
            _twitterRepository = twitterRepository;
        }

        public string GetName()
        {
            return "Assign graph data";
        }

        public async Task Act(int artistId)
        {
            await _twitterRepository.SetArtistTwitterDaySummary(artistId);
        }
    }
}
