using System.Collections.Generic;

namespace GameUtils
{
    public class Map<K, V> : Dictionary<K, V>
    {
        V defaultValue;

        public Map()
        {
            defaultValue = default(V);
        }

        public Map(V defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public Map(IEnumerable<KeyValuePair<K, V>> kvps)
        {
            foreach (var kvp in kvps)
                this[kvp.Key] = kvp.Value;
        }

        public Map(IEnumerable<KeyValuePair<K, V>> kvps, V defaultValue)
        {
            foreach (var kvp in kvps)
                this[kvp.Key] = kvp.Value;

            this.defaultValue = defaultValue;
        }

        public new V this[K key]
        {
            get
            {
                V value;
                var exists = TryGetValue(key, out value);
                return exists ? value : defaultValue;
            }

            set
            {
                if (ContainsKey(key))
                    base[key] = value;
                else
                    Add(key, value);
            }
        }

        public Map<K, V> Merge(IEnumerable<KeyValuePair<K, V>> other)
        {
            foreach (var kvp in other)
                this[kvp.Key] = kvp.Value;

            return this;
        }
    }
}
