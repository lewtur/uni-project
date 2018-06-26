namespace FYP.External.DataSources
{
    public interface ITwitterDataSourceConfig
    {
        int RateLimitWindowInMinutes { get; }
        int MaximumNumberStoredUnderOneEntity { get; }
        string CacheKey { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string BaseApiUrl { get; }
        string DateFormatInitial { get; }
        string DateFormatWithYear { get; }
    }

    public class TwitterDataSourceConfig : ITwitterDataSourceConfig
    {
        public int RateLimitWindowInMinutes => 15;
        public virtual int MaximumNumberStoredUnderOneEntity => 1000;
        public string CacheKey => "TwitterAuthToken";
        public string ClientId => "***REMOVED***";
        public string ClientSecret => "***REMOVED***";
        public string BaseApiUrl => "https://api.twitter.com";
        public string DateFormatInitial => "ddd MMM dd";
        public string DateFormatWithYear => DateFormatInitial + " yyyy";
    }
}