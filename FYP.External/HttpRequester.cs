using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FYP.External
{
    public class HttpRequester : IHttpRequester
    {
        private static HttpClient _client = new HttpClient();

        public T Get<T>(HttpClient client, string url)
        {
            try
            {
                var call = client.GetAsync(url);
                // call.Wait();

                if (call.Result.IsSuccessStatusCode)
                {
                    var content = call.Result.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<T>(content);
                }

                var resumeDate = TryGetResumeDateFromResponse(call.Result);
                if (resumeDate.Equals(DateTime.MinValue) )
                {
                    throw new Exception("Request failed and could not find a wait time, aborting.");
                }

                var delta = resumeDate - DateTime.UtcNow;

                if (delta.TotalMilliseconds < 0)
                {
                    // request failed for some other reason, just return
                    return default(T);
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Requester waiting {(int)delta.TotalMinutes}m{delta.Seconds}s");

                Task.Delay((int)delta.TotalMilliseconds).Wait();

                return Get<T>(client, url);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] HttpRequester threw an exception {e.Message}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Waiting 10s before starting again.");
                Task.Delay(10 * 1000).Wait();
                return default(T);
            }
           
        }

        public T Post<T>(HttpClient client, string url, HttpContent content)
        {
            var call = client.PostAsync(url, content);
            call.Wait();

            var result = call.Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(result);
        }

        public void ClearClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            //client.BaseAddress = new Uri();
        }

        public void AddHeader(HttpClient client, string key, string value)
        {
            client.DefaultRequestHeaders.Add(key, value);
        }

        private static DateTime TryGetResumeDateFromResponse(HttpResponseMessage response)
        {
            var twitterHeader = response.Headers.FirstOrDefault(x => x.Key.Equals("x-rate-limit-reset"));
            if (twitterHeader.Value != null)
            {
                var val = double.Parse(twitterHeader.Value.FirstOrDefault());
                return UnixTimeStampToDateTime(val);
            }

            var spotifyDelta = response.Headers?.RetryAfter?.Delta;
            if (spotifyDelta != null)
            {
                return DateTime.Now + spotifyDelta.Value;
            }

            return DateTime.MinValue;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public interface IHttpRequester
    {
        T Get<T>(HttpClient client, string url);
        T Post<T>(HttpClient client, string url, HttpContent content);
        void ClearClient(HttpClient client);
        void AddHeader(HttpClient client, string key, string value);
    }
}