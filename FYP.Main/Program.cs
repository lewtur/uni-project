using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Autofac;
using Dapper;
using FYP.Data;
using FYP.Data.DataSourceRepositories;
using FYP.External;
using FYP.External.DataSources;
using FYP.Main.Trends;
using FYP.Models;
using FYP.Models.Abstractions;
using FYP.Models.DataSourceRecords;
using FYP.Models.JsonModels;

namespace FYP.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("           Band updater");
            Console.WriteLine("===================================");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Options");
                Console.WriteLine(" a) Run events and data sources");
                Console.WriteLine(" b) Run data sources only");                
                Console.WriteLine(" c) Update popularity metrics");
                Console.WriteLine(" d) Update recent trending artists");
                Console.WriteLine(" t) Run test function");
                Console.WriteLine(" x) Exit");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "a":
                        IoC.Instance.Container.Resolve<IMainUpdater>().UpdateEventsAndDataSources();
                        Console.WriteLine("Events and data sources updated.");
                        break;
                    case "b":
                        IoC.Instance.Container.Resolve<IMainUpdater>().UpdateDataSources();
                        Console.WriteLine("Data sources updated.");
                        break;
                    case "t":
                        TestFunction();
                        Console.WriteLine("Tested.");
                        break;
                    case "c":
                        UpdatePopularityMetrics();
                        break;
                    case "d":
                        UpdateRecentTrendingArtists();
                        break;
                    case "x":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Try again...");
                        Console.WriteLine();
                        break;
                }
            }            
        }

        public static void UpdatePopularityMetrics()
        {
            var a = new PopularityFinder(
                IoC.Instance.Container.Resolve<IArtistFilterer>(),
                IoC.Instance.Container.Resolve<IGraphHunter>(),
                IoC.Instance.Container.Resolve<ITwitterDataSourceRepository>(),
                IoC.Instance.Container.Resolve<ISpotifyDataSourceRepository>(),
                new List<PopularityEvent>
                {
                    new AlbumReleasePopularityEvent(
                        IoC.Instance.Container.Resolve<ISpotifyDataSourceRepository>(),
                        new LinearPopularityConfig()
                    ),
                    new GigPopularityEvent(
                        IoC.Instance.Container.Resolve<IEventRepository>(),
                        new LinearPopularityConfig()
                    )
                },
                IoC.Instance.Container.Resolve<IPopularityRepository>()
            );

            a.FindAndUpdateAll().Wait();
        }

        private static void UpdateRecentTrendingArtists()
        {
            var popFinder = new PopularityFinder(
                IoC.Instance.Container.Resolve<IArtistFilterer>(),
                IoC.Instance.Container.Resolve<IGraphHunter>(),
                IoC.Instance.Container.Resolve<ITwitterDataSourceRepository>(),
                IoC.Instance.Container.Resolve<ISpotifyDataSourceRepository>(),
                new List<PopularityEvent>
                {
                    new AlbumReleasePopularityEvent(
                        IoC.Instance.Container.Resolve<ISpotifyDataSourceRepository>(),
                        new LinearPopularityConfig()
                    ),
                    new GigPopularityEvent(
                        IoC.Instance.Container.Resolve<IEventRepository>(),
                        new LinearPopularityConfig()
                    )
                },
                IoC.Instance.Container.Resolve<IPopularityRepository>()
            );

            var finder = new RecentTrendingBandFinder(new PopularityRepository(), popFinder,
                IoC.Instance.Container.Resolve<IArtistFilterer>());

            finder.AddTrendingArtists().Wait();

        }

        public static void TestFunction()
        {
            var a = IoC.Instance.Container.Resolve<IMainUpdater>();
        }        
    }    
}