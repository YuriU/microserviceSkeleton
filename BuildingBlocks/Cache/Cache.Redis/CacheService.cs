using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Cache.Redis
{
    public class CacheService : ICacheService
    {
        private IDatabase _database;
        
        public CacheService(string connectionString)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);
            _database = redis.GetDatabase();
        }

        public async Task Set<T>(string key, T value, TimeSpan ttl)
        {
            await _database.StringSetAsync(key, Serialize(value), ttl);
        }

        public async Task<CachedValue<T>> Get<T>(string key)
        {
            var strValue = await _database.StringGetAsync(key);
            return Deserialize<T>(strValue);
        }

        private string Serialize<T>(T value)
        {
            return typeof(T) == typeof(string)
                ? (string) (object) value
                : JsonConvert.SerializeObject(value);
        }

        private CachedValue<T> Deserialize<T>(string strValue)
        {
            if (strValue == null)
            {
                return CachedValue<T>.None;
            }

            if (typeof(T) == typeof(string))
            {
                return CachedValue<T>.FromValue((T) (object) strValue);
            }
            
            return CachedValue<T>.FromValue(JsonConvert.DeserializeObject<T>(strValue));
        }
    }
}