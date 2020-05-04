using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cache;
using Model.Donations;

namespace Services.Donations.Tests
{

    public class MockCacheService : ICacheService
    {
        public class DictionaryKey
        {
            protected bool Equals(DictionaryKey other)
            {
                return Key == other.Key && TypeName == other.TypeName;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DictionaryKey) obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, TypeName);
            }

            public DictionaryKey(string key, string typeName)
            {
                Key = key;
                TypeName = typeName;
            }

            public string Key { get; }
            
            public string TypeName { get; }
            
        }
        
        public Dictionary<DictionaryKey, object> Dictionary = new Dictionary<DictionaryKey, object>();
        
        public Task Set<T>(string key, T value, TimeSpan ttl)
        {
            Dictionary.Add(new DictionaryKey(key, typeof(T).FullName), value);
            return Task.CompletedTask;
        }

        public Task<CachedValue<T>> Get<T>(string key)
        {
            if (Dictionary.TryGetValue(new DictionaryKey(key, typeof(T).FullName), out var value))
            {
                return Task.FromResult(CachedValue<T>.FromValue((T)value));
            }

            return Task.FromResult(CachedValue<T>.None);

        }
    }
}