using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.Models.Abstractions;

namespace FYP.External.DataSources
{
    public class ArtistTwitterDataSource : TwitterDataSource
    {
        public ArtistTwitterDataSource(ITwitterDataSourceRepository repository, IHttpRequester httpRequester, ICache cache, ITwitterDataSourceConfig config,
            ITwitterLimitRepository limitRepository)
            : base(repository, httpRequester, cache, config, limitRepository)
        {
        }

        public override bool IsArtist => true;
    }
}