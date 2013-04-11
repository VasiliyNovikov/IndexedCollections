namespace IndexedCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BalancedTreeLeafNode<TKey, TValue> : BalancedTreeNode<TKey, TValue, BalancedTreeLeafNodeEntry<TKey, TValue>>
    {
        public BalancedTreeLeafNode(BalancedTreeConfig<TKey> config)
            : base(config)
        {
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.ActiveEntries.Select(e => new KeyValuePair<TKey, TValue>(e.Key, e.Value)).GetEnumerator();
        }

        public override void Add(TKey key, TValue value)
        {
            this.CheckIsFull();

            bool isEqual;
            var insertPosition = this.FindFirstEqualOrGreater(key, out isEqual);
            if (isEqual)
            {
                throw new ArgumentException();
            }

            this.InsertEntry(insertPosition, new BalancedTreeLeafNodeEntry<TKey, TValue>(key, value));
        }

        public override bool Remove(TKey key)
        {
            bool isEqual;
            var removePosition = this.FindFirstEqualOrGreater(key, out isEqual);
            if (!isEqual)
            {
                return false;
            }

            this.RemoveEntry(removePosition);
            return true;
        }

        protected override BalancedTreeNode<TKey, TValue, BalancedTreeLeafNodeEntry<TKey, TValue>> CloneStructure()
        {
            return new BalancedTreeLeafNode<TKey, TValue>(this.Config);
        }

        #region Iterators support

        public override Iterator CreateIterator()
        {
            return new LeafNodeIterator();
        }

        public override void MoveToBegin(Iterator iterator)
        {
            ((LeafNodeIterator)iterator).Init(this);
        }

        public override void MoveToEnd(Iterator iterator)
        {
            ((LeafNodeIterator)iterator).Init(this, this.Count);
        }

        public override void FindFirstEqualOrGreater(Iterator iterator, TKey key, out bool isEqual)
        {
            var comparer = this.Config.KeyComparer;
            var compareResult = -1;
            while (!iterator.IsEnd && (compareResult = comparer.Compare(iterator.Key, key)) < 0)
            {
                iterator.MoveNext();
            }

            isEqual = compareResult == 0;
        }

        public override void InsertEntry(Iterator iterator, TKey key, TValue value)
        {
            this.CheckIsFull();

            var leafNodeIterator = (LeafNodeIterator)iterator;
            var insertPosition = leafNodeIterator.Position;

            this.InsertEntry(insertPosition, new BalancedTreeLeafNodeEntry<TKey, TValue>(key, value));
            leafNodeIterator.Init(this, insertPosition);
        }

        private class LeafNodeIterator : Iterator
        {
            private BalancedTreeLeafNode<TKey, TValue> node;

            private BalancedTreeLeafNodeEntry<TKey, TValue>[] entries;

            private int count;

            public void Init(BalancedTreeLeafNode<TKey, TValue> initNode, int initPosition = 0)
            {
                this.Init();
                this.node = initNode;
                this.count = initNode.Count;
                this.entries = initNode.Entries;
                this.Position = initPosition;
            }

            public int Position { get; private set; }

            public override bool IsEnd
            {
                get
                {
                    this.CheckDisposed();
                    return this.Position == this.count;
                }
            }

            public override TKey Key
            {
                get
                {
                    this.CheckValid();
                    return this.entries[this.Position].Key;
                }
            }

            public override TValue Value
            {
                get
                {
                    this.CheckValid();
                    return this.entries[this.Position].Value;
                }
                set
                {
                    this.CheckValid();
                    this.entries[this.Position].Value = value;
                }
            }

            public override BalancedTreeIterator<TKey, TValue> Clone()
            {
                this.CheckValid();
                var newIterator = new LeafNodeIterator { node = this.node, entries = this.entries, Position = this.Position, count = this.count };
                newIterator.Init();
                return newIterator;
            }

            public override bool MoveNext()
            {
                this.CheckValid();
                ++this.Position;
                return !this.IsEnd;
            }

            public override void Remove()
            {
                this.CheckValid();
                this.node.RemoveEntry(this.Position);
                this.Dispose();
            }

            public override void Dispose()
            {
                base.Dispose();
                this.entries = null;
            }
        }

        #endregion
    }
}