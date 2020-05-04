using System;
using System.Threading.Tasks;

namespace Cache
{
    public interface ICacheService
    {
        Task Set<T>(string key, T value, TimeSpan ttl);

        Task<CachedValue<T>> Get<T>(string key);
    }
}