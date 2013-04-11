namespace IndexedCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Balanced tree with support add, remove, key oredering, nvigation operations
    /// </summary>
    public class BalancedTree<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Constants and Fields

        private static readonly IEqualityComparer<TValue> ValueComparer = EqualityComparer<TValue>.Default;

        /// <summary>
        /// Balanced tree configuration
        /// </summary>
        private readonly BalancedTreeConfig<TKey> config;

        /// <summary>
        /// Root node
        /// </summary>
        private BalancedTreeNode<TKey, TValue> root;

        #endregion

        #region Constructors and Destructors

        public BalancedTree(int minNodeSize, bool isReversed = false)
        {
            this.config = new BalancedTreeConfig<TKey>(minNodeSize, isReversed);
        }

        public BalancedTree(int minNodeSize, IEnumerable<KeyValuePair<TKey, TValue>> source, bool isReversed = false)
            : this(minNodeSize, isReversed)
        {
            foreach (var kvp in source)
            {
                this.Add(kvp.Key, kvp.Value);
            }
        }

        #endregion

        #region Properties

        public int Count { get; private set; }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Indexers

        public TValue this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICollection<KeyValuePair<TKey,TValue>>

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.root = null;
            this.Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return this.TryGetValue(item.Key, out value) && ValueComparer.Equals(item.Value, value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDictionary<TKey,TValue>

        public void Add(TKey key, TValue value)
        {
            if (this.root == null)
            {
                this.root = new BalancedTreeLeafNode<TKey, TValue>(this.config);
            }

            this.root.Add(key, value);
            ++this.Count;

            if (!this.root.IsFull)
            {
                return;
            }

            // Balancing
            var newRoot = new BalancedTreeInnerNode<TKey, TValue>(this.root, this.root.Split());
            this.root = newRoot;
        }

        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            if (this.root == null)
            {
                return false;
            }

            bool result = this.root.Remove(key);
            if (result)
            {
                --this.Count;
            }

            if (!result || this.root.Count > 1)
            {
                return result;
            }

            // Balancing
            if (this.root.Count == 1)
            {
                var innerNode = this.root as BalancedTreeInnerNode<TKey, TValue>;
                if (innerNode != null)
                {
                    this.root = innerNode.FirstChild;
                    innerNode.ClearNoChildren();
                }
            }
            else
            {
                this.root = null;
            }

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (this.root ?? Enumerable.Empty<KeyValuePair<TKey, TValue>>()).GetEnumerator();
        }

        #endregion

        #endregion
    }
}