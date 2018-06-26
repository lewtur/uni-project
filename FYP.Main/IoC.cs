using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Main.PostUpdateActions;
using FYP.Main.Trends;
using FYP.Models;
using FYP.Models.Abstractions;

namespace FYP.Main
{
    public class IoC
    {
        private static IoC _instance;

        private IoC()
        {
            var builder = RegisterBuilder();

            Container = builder.Build();
        }

        public static IoC Instance => _instance ?? (_instance = new IoC());

        public IContainer Container { get; set; }

        private ContainerBuilder RegisterBuilder()
        {
            var builder = new ContainerBuilder();

            builder.Register<IHttpRequester>(x => new HttpRequester());
            builder.Register<IArtistRepository>(x => new ArtistRepository());
            builder.Register<IEventRepository>(x => new EventRepository());
            builder.Register<IVenueRepository>(x => new VenueRepository());
            builder.Register<ILogger>(x => new ConsoleLogger());
            builder.Register<ITwitterLimitRepository>(x => new TwitterLimitRepository());
            builder.Register<IInvalidSpotifyRepository>(x => new InvalidSpotifyRepository());
            builder.Register<IPopularityRepository>(x => new PopularityRepository());
            builder.Register<IGraphHunter>(x => new GraphHunter());
            builder.Register<ITwitterDataSourceRepository>(x => new TwitterDataSourceRepository());
            builder.Register<ISpotifyDataSourceRepository>(x => new SpotifyDataSourceRepository());
            builder.Register<IGenreRepository>(x => new GenreRepository());

            builder.Register<IArtistInfoRetriever>(x => new ArtistInfoRetriever(x.Resolve<IHttpRequester>()));
            builder.Register<IEventsRetriever>(x => new EventsRetriever(x.Resolve<IHttpRequester>()));
            builder.Register<ISpotifyCredentials>(
                x => new SpotifyCredentials(MemoryCache.Instance, x.Resolve<IHttpRequester>()));
            builder.Register<IArtistFilterer>(x => new ArtistFilterer(
                x.Resolve<IArtistRepository>(),
                x.Resolve<ITwitterLimitRepository>(),
                x.Resolve<IInvalidSpotifyRepository>()
            ));            
            builder.Register<IVenueFilterer>(
                x => new VenueFilterer(x.Resolve<IVenueRepository>(), x.Resolve<ITwitterLimitRepository>()));
            builder.Register<ISpotifyCredentials>(
                x => new SpotifyCredentials(MemoryCache.Instance, x.Resolve<IHttpRequester>()));            
            builder.Register<IEventUpdater>(x => new EventUpdater(
                x.Resolve<IArtistRepository>(),
                x.Resolve<IEventRepository>(),
                x.Resolve<IVenueRepository>(),
                x.Resolve<IEventsRetriever>(),
                new List<Location>
                {
                    new Location {Name = "Manchester", Longitude = "-2.2446", Latitude = "53.4839", Radius = "6"},
                    new Location {Name = "Birmingham", Longitude = "-1.8904", Latitude = "52.4862", Radius = "6"},
                    new Location {Name = "Leeds", Longitude = "-1.5491", Latitude = "53.8008", Radius = "6"},
                    new Location {Name = "Bristol", Longitude = "-2.5879", Latitude = "51.4545", Radius = "6"},
                    new Location {Name = "Sheffield", Longitude = "-1.4701", Latitude = "53.3811", Radius = "6"},
                    new Location {Name = "Liverpool", Longitude = "-2.9916", Latitude = "53.4084", Radius = "6"},
                    new Location {Name = "London", Longitude = "-0.1278", Latitude = "51.5074", Radius = "15"},
                    new Location {Name = "Glasgow", Longitude = "-4.2518", Latitude = "55.8642", Radius = "6"},
                    new Location {Name = "Cardiff", Longitude = "-3.1791", Latitude = "51.4816", Radius = "6"}
                },
                x.Resolve<IArtistInfoRetriever>())                
            );
            builder.Register<ITwitterDataSourceConfig>(x => new TwitterDataSourceConfig());

            builder.Register<IMainUpdater>(x => new MainUpdater(
                new List<IDataSource>
                {
                    new SpotifyDataSource(
                        x.Resolve<ISpotifyDataSourceRepository>(),
                        x.Resolve<IHttpRequester>(),
                        x.Resolve<IInvalidSpotifyRepository>(),
                        x.Resolve<IGenreRepository>(),
                        x.Resolve<ISpotifyCredentials>()
                    ),
                    new SpotifyArtistImageDataSource(
                        x.Resolve<IHttpRequester>(),
                        x.Resolve<ISpotifyDataSourceRepository>(),
                        x.Resolve<IArtistRepository>(),
                        x.Resolve<ISpotifyCredentials>()
                    ),
                    new ArtistDescriptionDataSource(
                        x.Resolve<IArtistRepository>(),
                        x.Resolve<IArtistInfoRetriever>()
                    ),
                    new ArtistTwitterDataSource(
                        x.Resolve<ITwitterDataSourceRepository>(),
                        x.Resolve<IHttpRequester>(),
                        MemoryCache.Instance,
                        x.Resolve<ITwitterDataSourceConfig>(),
                        x.Resolve<ITwitterLimitRepository>()
                    )
                },
                new List<IDataSource>
                {
                    //new VenueTwitterDataSource(new TwitterDataSourceRepository(), new HttpRequester(), MemoryCache.Instance, new TwitterDataSourceConfig(), new TwitterLimitRepository())
                },
                new List<IPostUpdateAction>
                {
                    new AssignStatsToArtists(new TwitterDataSourceRepository())
                },
                x.Resolve<IEventUpdater>(),
                x.Resolve<IArtistFilterer>(),
                x.Resolve<IVenueFilterer>(),
                x.Resolve<ILogger>()));

            return builder;
        }        
    }
}
