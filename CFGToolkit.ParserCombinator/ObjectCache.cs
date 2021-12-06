using System;
using System.Collections.Concurrent;

namespace CFGToolkit.ParserCombinator
{
    public class ObjectCache
    {
        public static ConcurrentDictionary<object, object> _singleKeyCache = new ConcurrentDictionary<object, object>();
        public static ConcurrentDictionary<(object, object), object> _doubleCache = new ConcurrentDictionary<(object, object), object>();
        public static ConcurrentDictionary<(object, object, object), object> _tripple = new ConcurrentDictionary<(object, object, object), object>();

        public static TValue CacheGet<TKey, TValue>(TKey key, Func<TValue> factory)
        {
            if (!_singleKeyCache.TryGetValue(key, out var result))
            {
                var val = factory();
                _singleKeyCache[key] = val;
                return val;
            }
            else
            {
                return (TValue)result;
            }
        }

        public static TValue CacheGet<TKey, TKey2, TValue>(TKey key, TKey2 key2, Func<TValue> factory)
        {
            if (!_doubleCache.TryGetValue((key, key2), out var result))
            {
                var val = factory();
                _doubleCache[(key, key2)] = val;
                return val;
            }
            else
            {
                return (TValue)result;
            }
        }


        public static TValue CacheGet<TKey, TKey2, TKey3, TValue>(TKey key, TKey2 key2, TKey3 key3, Func<TValue> factory)
        {
            if (!_tripple.TryGetValue((key, key2, key3), out var result))
            {
                var val = factory();
                _tripple[(key, key2, key3)] = val;
                return val;
            }
            else
            {
                return (TValue)result;
            }
        }

        public static void Clear()
        {
            _singleKeyCache.Clear();
            _doubleCache.Clear();
            _tripple.Clear();
        }
    }
}
