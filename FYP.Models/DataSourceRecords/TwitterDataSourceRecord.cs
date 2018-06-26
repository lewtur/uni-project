using System;
using System.Collections.Generic;
using System.Text;

namespace FYP.Models.DataSourceRecords
{
    public class TwitterUser
    {
        public int Id { get; set; }
        public string TwitterId { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public string Location { get; set; }
        public string UserDescription { get; set; }
        public string DateSignedUp { get; set; }
    }

    public class Tweet
    {
        public int Id { get; set; }
        public int? ArtistId { get; set; }
        public int? VenueId { get; set; }
        public int TwitterUserId { get; set; }
        public string DateCreated { get; set; }
        public DateTime DateSavedInDb { get; set; }
        public string TwitterId { get; set; }
        public string Text { get; set; }
        public int RetweetCount { get; set; }
        public int FavouriteCount { get; set; }
        public string Language { get; set; }
    }

    public class TwitterUserTimestamp
    {
        public int Id { get; set; }
        public int TwitterUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public int FollowersCount { get; set; }
        public int FriendsCount { get; set; }
        public int ListedCount { get; set; }
        public int FavouritesCount { get; set; }
        public bool Verified { get; set; }
        public int StatusesCount { get; set; }
    }
}