using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using FYP.Models.Abstractions;

namespace FYP.Main
{
    public class MemoryCache : ICache
    {
        private static MemoryCache _instance;
        private readonly Dictionary<string, Tuple<string, DateTime>> _table;

        private MemoryCache()
        {
            _table = new Dictionary<string, Tuple<string, DateTime>>();
        }

        public static MemoryCache Instance => _instance ?? (_instance = new MemoryCache());

        public void Add(string item, string key, DateTime dateExpires)
        {
            _table.TryGetValue(key, out Tuple<string, DateTime> value);

            if (value == null)
            {
                _table.Add(key, new Tuple<string, DateTime>(item, dateExpires));
            }

            _table[key] = new Tuple<string, DateTime>(item, dateExpires);
        }

        public void Remove(string key)
        {
            _table.TryGetValue(key, out Tuple<string, DateTime> value);

            if (value != null)
            {
                _table.Remove(key);
            }
        }

        public string Get(string key)
        {
            _table.TryGetValue(key, out Tuple<string, DateTime> value);

            if (value == null) return null;

            if (value.Item2 > DateTime.Now) return value.Item1;

            Remove(key);
            return null;
        }
    }
}
