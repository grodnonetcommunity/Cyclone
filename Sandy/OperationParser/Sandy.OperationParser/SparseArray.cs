using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public class SparseArray<T> : IEnumerable<KeyValuePair<int, T>>
    {
        private readonly Dictionary<int, T> array = new Dictionary<int, T>();
        private readonly Func<int, T> activator;

        public SparseArray() : this(null)
        {
        }

        public SparseArray(Func<int, T> activator)
        {
            this.activator = activator;
        }

        public T this[int index]
        {
            get { return GetOrAddInternal(index, activator); }
            set { array[index] = value; }
        }

        public T GetOrAdd(int index, [NotNull] Func<int, T> factory)
        {
            if (factory == null) 
                throw new ArgumentNullException("factory");
            return GetOrAddInternal(index, factory);
        }

        public int Count
        {
            get { return array.Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        private T GetOrAddInternal(int index, Func<int, T> factory)
        {
            T value;
            if (array.TryGetValue(index, out value) || factory == null) return value;
            value = factory(index);
            array[index] = value;
            return value;
        }
    }
}