using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.JsonModels
{
    public class SpotifyAuthentication
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
