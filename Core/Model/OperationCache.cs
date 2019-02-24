using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides a basic in-memory stage cache for objects associated with an IOperation.
    /// </summary>
    public class OperationCache
    {
        /// <summary>
        /// Gets the IOperation associated with the cache.
        /// </summary>
        public IOperation Operation { get; private set; }

        /// <summary>
        /// Initialises a new instance of the OperationCache class for the specified IOperation.
        /// </summary>
        /// <param name="operation">The Connection.</param>
        public OperationCache(IOperation operation)
        {
            Operation = operation;
        }

        /// <summary>
        /// Gets or sets the cached object with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cached object.</returns>
        public object this[string key]
        {
            get { return MemoryCache.Default[GetCacheKey(key)]; }
            set { MemoryCache.Default[GetCacheKey(key)] = value; }
        }

        /// <summary>
        /// Returns the number of items in the cache.
        /// </summary>
        /// <returns>The item count.</returns>
        public int Count
        {
            get
            {
                int itemCount = 0;
                ObjectCache cache = MemoryCache.Default;
                List<string> keys = cache.Select(key => key.Key).ToList();
                string prefix = GetCacheKeyPrefix();

                foreach (string key in keys)
                {
                    if (key.StartsWith(prefix))
                    {
                        itemCount++;
                    }
                }

                return itemCount;
            }
        }

        /// <summary>
        /// Clears all cached objects.
        /// </summary>
        public void Clear()
        {
            ObjectCache cache = MemoryCache.Default;
            List<string> keys = cache.Select(key => key.Key).ToList();
            string prefix = GetCacheKeyPrefix();

            foreach (string key in keys)
            {
                if (key.StartsWith(prefix))
                {
                    cache.Remove(key);
                }
            }
        }

        private string GetCacheKeyPrefix()
        {
            return string.Format("OperationCache:{0}", Operation.ID.ToString());
        }

        public string GetCacheKey(string key)
        {
            string prefix = GetCacheKeyPrefix();
            return string.Format(string.Concat(prefix, ":{0}"), key);
        }
    }
}
