namespace IndexedCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BalancedTreeNode<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        #region Constants and Fields

        public readonly BalancedTreeConfig<TKey> Config;

        #endregion

        #region Constructors and Destructors

        protected BalancedTreeNode(BalancedTreeConfig<TKey> config)
        {
            this.Config = config;
        }

        #endregion

        #region Properties

        public int Count { get; protected set; }

        public abstract TKey FirstKey { get; }

        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return this.Count == this.Config.Capacity;
            }
        }

        public bool IsMinCount
        {
            get
            {
                return this.Count == this.Config.MinNodeSize;
            }
        }

        public bool IsUnderfull
        {
            get
            {
                return this.Count < this.Config.MinNodeSize;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Add(TKey key, TValue value);

        public abstract void Append(BalancedTreeNode<TKey, TValue> node);

        public abstract void MoveFirstTo(BalancedTreeNode<TKey, TValue> node);

        public abstract void MoveLastTo(BalancedTreeNode<TKey, TValue> node);

        public abstract bool Remove(TKey key);

        public abstract BalancedTreeNode<TKey, TValue> Split();

        #endregion

        #region Iterators support

        public abstract Iterator CreateIterator();

        public abstract void MoveToBegin(Iterator iterator);
        
        public abstract void MoveToEnd(Iterator iterator);

        public abstract void FindFirstEqualOrGreater(Iterator iterator, TKey key, out bool isEqual);

        public abstract void InsertEntry(Iterator iterator, TKey key, TValue value);

        #endregion

        #region Implemented Interfaces

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>>

        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        #endregion

        #endregion

        #region Methods

        protected void CheckIsFull()
        {
            if (this.IsFull)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion

        public abstract class Iterator : BalancedTreeIterator<TKey, TValue>
        {
            private bool isDisposed = true;

            public override void Dispose()
            {
                this.isDisposed = true;
            }

            public override bool IsDisposed
            {
                get
                {
                    return this.isDisposed;
                }
            }

            protected void Init()
            {
                this.isDisposed = false;
            }

            protected void CheckDisposed()
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException("Iterator");
                }
            }

            protected void CheckEnd()
            {
                if (this.IsEnd)
                {
                    throw new InvalidOperationException();
                }
            }

            protected void CheckValid()
            {
                this.CheckDisposed();
                this.CheckEnd();
            }
        }
    }

    public abstract class BalancedTreeNode<TKey, TValue, TEntry> : BalancedTreeNode<TKey, TValue>
        where TEntry : struct, IBalancedTreeNodeEntry<TKey>
    {
        #region Constants and Fields

        protected readonly TEntry[] Entries;

        #endregion

        #region Constructors and Destructors

        protected BalancedTreeNode(BalancedTreeConfig<TKey> config)
            : base(config)
        {
            this.Entries = new TEntry[config.Capacity];
        }

        #endregion

        #region Properties

        public override TKey FirstKey
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException();
                }

                return this.Entries[0].Key;
            }
        }

        protected IEnumerable<TEntry> ActiveEntries
        {
            get
            {
                return this.Entries.Take(this.Count);
            }
        }

        #endregion

        #region Public Methods

        public override void Append(BalancedTreeNode<TKey, TValue> node)
        {
            int count = this.Count;
            var appendNode = (BalancedTreeNode<TKey, TValue, TEntry>)node;
            int appendCount = appendNode.Count;
            BalancedTreeConfig<TKey> config = this.Config;

            if (count + appendCount > config.Capacity)
            {
                throw new InvalidOperationException();
            }

            TEntry[] appendEntries = appendNode.Entries;
            Array.Copy(appendEntries, 0, this.Entries, count, appendCount);
            Array.Clear(appendEntries, 0, appendCount);
            this.Count = count + appendCount;
            appendNode.Count = 0;
        }

        public override void MoveFirstTo(BalancedTreeNode<TKey, TValue> node)
        {
            var targetNode = (BalancedTreeNode<TKey, TValue, TEntry>)node;
            if (this.IsUnderfull || targetNode.IsFull)
            {
                throw new InvalidOperationException();
            }

            targetNode.Entries[targetNode.Count++] = this.Entries[0];
            this.RemoveEntry(0);
        }

        public override void MoveLastTo(BalancedTreeNode<TKey, TValue> node)
        {
            var targetNode = (BalancedTreeNode<TKey, TValue, TEntry>)node;
            if (this.IsUnderfull || targetNode.IsFull)
            {
                throw new InvalidOperationException();
            }

            int newCount = this.Count - 1;
            TEntry[] entries = this.Entries;
            targetNode.InsertEntry(0, entries[newCount]);
            this.Count = newCount;
            entries[newCount] = default(TEntry);
        }

        public override BalancedTreeNode<TKey, TValue> Split()
        {
            if (!this.IsFull)
            {
                throw new InvalidOperationException();
            }

            BalancedTreeConfig<TKey> config = this.Config;
            TEntry[] entries = this.Entries;
            BalancedTreeNode<TKey, TValue, TEntry> newNode = this.CloneStructure();
            int newCount = config.MinNodeSize;
            int newNodeCount = config.Capacity - newCount;
            Array.Copy(entries, newCount, newNode.Entries, 0, newNodeCount);
            Array.Clear(entries, newCount, newNodeCount);
            this.Count = newCount;
            newNode.Count = newNodeCount;
            return newNode;
        }

        #endregion

        #region Methods

        protected abstract BalancedTreeNode<TKey, TValue, TEntry> CloneStructure();

        protected int FindFirstEqualOrGreater(TKey key, out bool isEqual)
        {
            var count = this.Count;
            var config = this.Config;
            var comparer = config.KeyComparer;
            var entries = this.Entries;
            var index = 0;
            var compareResult = -1;
            while (index < count && (compareResult = comparer.Compare(entries[index].Key, key)) < 0)
            {
                ++index;
            }

            isEqual = compareResult == 0;
            return index;
        }

        protected int FindFirstGreater(TKey key, bool throwIfKeyExists = true)
        {
            var count = this.Count;
            var config = this.Config;
            var comparer = config.KeyComparer;
            var entries = this.Entries;
            var index = 0;
            int compareResult;
            while (index < count && (compareResult = comparer.Compare(entries[index].Key, key)) <= 0)
            {
                if (compareResult == 0 && throwIfKeyExists)
                {
                    throw new ArgumentException("Key already exists");
                }

                ++index;
            }

            return index;
        }

        protected void InsertEntry(int index, TEntry entry)
        {
            this.CheckIsFull();

            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            TEntry[] entries = this.Entries;
            int count = this.Count;
            for (int i = count; i > index; --i)
            {
                entries[i] = entries[i - 1];
            }

            entries[index] = entry;
            this.Count = count + 1;
        }

        protected void RemoveEntry(int index)
        {
            int count = this.Count;
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int newCount = count - 1;
            TEntry[] entries = this.Entries;
            for (int i = index; i < newCount; ++i)
            {
                entries[i] = entries[i + 1];
            }

            this.Count = newCount;
            entries[newCount] = default(TEntry);
        }

        #endregion
    }
}