using System;

namespace FYP.Models.Abstractions
{
    public interface ICache
    {
        void Add(string item, string key, DateTime dateExpires);
        void Remove(string key);
        string Get(string key);
    }
}