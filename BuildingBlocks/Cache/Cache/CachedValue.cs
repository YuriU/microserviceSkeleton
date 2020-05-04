namespace Cache
{
    public class CachedValue<T>
    {
        public bool Exists { get; }
        
        public T Value { get; }

        private CachedValue(bool exists, T value)
        {
            Exists = exists;
            Value = value;
        }

        public static CachedValue<T> FromValue(T value)
        {
            return new CachedValue<T>(true, value);
        }
        
        public static CachedValue<T> None => new CachedValue<T>(false, default(T));
    }
}