using System.Collections.Generic;

namespace GameUtils
{
    public class Set<T> : HashSet<T>
    {
        public Set()
        {
        }

        public Set(IEnumerable<T> set)
        {
            foreach (var e in set)
                this.Add(e);
        }

        public bool this[T elem]
        {
            get { return Contains(elem); }
        }

        public new Set<T> Add(T elem)
        {
            this.Add(elem);
            return this;
        }

        public new Set<T> Remove(T elem)
        {
            this.Remove(elem);
            return this;
        }

        public Set<T> Merge(IEnumerable<T> other)
        {
            foreach (var elem in other)
                Add(elem);

            return this;
        }
    }
}
