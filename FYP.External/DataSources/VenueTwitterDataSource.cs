using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models.Abstractions;

namespace FYP.External.DataSources
{
    public class VenueTwitterDataSource : TwitterDataSource
    {
        public VenueTwitterDataSource(ITwitterDataSourceRepository repository, IHttpRequester httpRequester, ICache cache, ITwitterDataSourceConfig config,
            ITwitterLimitRepository limitRepository)
            : base(repository, httpRequester, cache, config, limitRepository)
        {
        }

        public override bool IsArtist => false;
    }
}