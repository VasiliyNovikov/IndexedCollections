namespace IndexedCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BalancedTreeInnerNode<TKey, TValue> : BalancedTreeNode<TKey, TValue, BalancedTreeInnerNodeEntry<TKey, TValue>>
    {
        public BalancedTreeInnerNode(BalancedTreeConfig<TKey> config)
            : base(config)
        {
        }

        public BalancedTreeInnerNode(BalancedTreeNode<TKey, TValue> leftNode, BalancedTreeNode<TKey, TValue> rightNode)
            : base(leftNode.Config)
        {
            this.Entries[0] = new BalancedTreeInnerNodeEntry<TKey, TValue>(leftNode);
            this.Entries[1] = new BalancedTreeInnerNodeEntry<TKey, TValue>(rightNode);
            this.Count = 2;
        }

        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ActiveEntries.SelectMany(e => e.ChildNode).GetEnumerator();
        }

        public override void Add(TKey key, TValue value)
        {
            this.CheckIsFull();

            // Adding
            var index = this.FindFirstGreater(key) - 1;
            if (index < 0)
            {
                this.Entries[0].Key = key;
                index = 0;
            }

            var childNode = this.Entries[index].ChildNode;
            childNode.Add(key, value);

            if (!childNode.IsFull)
            {
                return;
            }

            // Balancing
            this.InsertEntry(index + 1, new BalancedTreeInnerNodeEntry<TKey, TValue>(childNode.Split()));
        }

        public override bool Remove(TKey key)
        {
            bool isEqual;
            var removePosition = this.FindFirstEqualOrGreater(key, out isEqual);
            if (!isEqual)
            {
                --removePosition;
            }

            if (removePosition < 0)
            {
                throw new InvalidOperationException();
            }

            var entries = this.Entries;
            var childNode = entries[removePosition].ChildNode;
            var result = childNode.Remove(key);
            if (!result)
            {
                return false;
            }

            if (removePosition == 0)
            {
                entries[0].Key = childNode.FirstKey;
            }

            // Balancing
            if (!childNode.IsUnderfull)
            {
                return true;
            }

            BalancedTreeNode<TKey, TValue> prevNode = null;
            BalancedTreeNode<TKey, TValue> nextNode = null;

            if (removePosition > 0)
            {
                prevNode = entries[removePosition - 1].ChildNode;
                if (!prevNode.IsMinCount)
                {
                    prevNode.MoveLastTo(childNode);
                    entries[removePosition].Key = childNode.FirstKey;
                    return true;
                }
            }

            if (removePosition < this.Count - 1)
            {
                nextNode = entries[removePosition + 1].ChildNode;
                if (!nextNode.IsMinCount)
                {
                    nextNode.MoveFirstTo(childNode);
                    entries[removePosition + 1].Key = nextNode.FirstKey;
                    return true;
                }
            }

            if (prevNode != null)
            {
                prevNode.Append(childNode);
                this.RemoveEntry(removePosition);
            }
            else
            {
                childNode.Append(nextNode);
                this.RemoveEntry(removePosition + 1);
            }

            return true;
        }

        public BalancedTreeNode<TKey, TValue> FirstChild
        {
            get
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException();
                }

                return this.Entries[0].ChildNode;
            }
        }

        public void ClearNoChildren()
        {
            Array.Clear(this.Entries, 0, this.Count);
            this.Count = 0;
        }

        protected override BalancedTreeNode<TKey, TValue, BalancedTreeInnerNodeEntry<TKey, TValue>> CloneStructure()
        {
            return new BalancedTreeInnerNode<TKey, TValue>(this.Config);
        }

        #region Iterators support

        public override Iterator CreateIterator()
        {
            return new InnerNodeIterator();
        }

        public override void MoveToBegin(Iterator iterator)
        {
            var firstChildNode = this.Entries[0].ChildNode;
            var childIterator = firstChildNode.CreateIterator();
            firstChildNode.MoveToBegin(childIterator);
            ((InnerNodeIterator)iterator).Init(this, childIterator); 
        }

        public override void MoveToEnd(Iterator iterator)
        {
            var endPosition = this.Count - 1;
            var lastChildNode = this.Entries[endPosition].ChildNode;
            var childIterator = lastChildNode.CreateIterator();
            lastChildNode.MoveToEnd(childIterator);
            ((InnerNodeIterator)iterator).Init(this, childIterator, endPosition); 
        }

        public override void FindFirstEqualOrGreater(Iterator iterator, TKey key, out bool isEqual)
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
            if (index == count)
            {
                this.MoveToEnd(iterator);
            }
            else
            {
                
            }
            //return index;
            throw new NotImplementedException();
        }

        public override void InsertEntry(Iterator iterator, TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        private class InnerNodeIterator : Iterator
        {
            private BalancedTreeInnerNode<TKey, TValue> node;

            private BalancedTreeInnerNodeEntry<TKey, TValue>[] entries;

            private int count;

            private Iterator childIterator;

            private int position;

            public void Init(BalancedTreeInnerNode<TKey, TValue> initNode, Iterator initChildIterator, int initPosition = 0)
            {
                this.Init();
                this.node = initNode;
                this.count = initNode.Count;
                this.entries = initNode.Entries;
                this.childIterator = initChildIterator;
                this.position = initPosition;
            }

            public override bool IsEnd
            {
                get
                {
                    this.CheckDisposed();
                    return this.childIterator.IsEnd;
                }
            }

            public override TKey Key
            {
                get
                {
                    this.CheckValid();
                    return this.childIterator.Key;
                }
            }

            public override TValue Value
            {
                get
                {
                    this.CheckValid();
                    return this.childIterator.Value;
                }
                set
                {
                    this.CheckValid();
                    this.childIterator.Value = value;
                }
            }

            public override BalancedTreeIterator<TKey, TValue> Clone()
            {
                this.CheckValid();
                return new InnerNodeIterator
                    {
                        node = this.node,
                        entries = this.entries,
                        count = this.count,
                        childIterator = this.childIterator,
                        position = this.position
                    };
            }

            public override bool MoveNext()
            {
                this.CheckValid();
                if (this.childIterator.MoveNext())
                {
                    return true;
                }

                var newPosition = this.position + 1;
                if (newPosition == this.count)
                {
                    return false;
                }

                this.entries[newPosition].ChildNode.MoveToBegin(this.childIterator);
                this.position = newPosition;
                return true;
            }

            public override void Remove()
            {
                this.CheckValid();

                this.childIterator.Remove();
                var positionCopy = this.position;
                var entriesCopy = this.entries;
                this.Dispose();

                var childNode = entriesCopy[positionCopy].ChildNode;

                if (positionCopy == 0)
                {
                    entriesCopy[0].Key = childNode.FirstKey;
                }

                // Balancing
                if (!childNode.IsUnderfull)
                {
                    return;
                }

                BalancedTreeNode<TKey, TValue> prevNode = null;
                BalancedTreeNode<TKey, TValue> nextNode = null;

                if (positionCopy > 0)
                {
                    prevNode = entriesCopy[positionCopy - 1].ChildNode;
                    if (!prevNode.IsMinCount)
                    {
                        prevNode.MoveLastTo(childNode);
                        entriesCopy[positionCopy].Key = childNode.FirstKey;
                        return;
                    }
                }

                if (positionCopy < this.count - 1)
                {
                    nextNode = entriesCopy[positionCopy + 1].ChildNode;
                    if (!nextNode.IsMinCount)
                    {
                        nextNode.MoveFirstTo(childNode);
                        entriesCopy[positionCopy + 1].Key = nextNode.FirstKey;
                        return;
                    }
                }

                if (prevNode != null)
                {
                    prevNode.Append(childNode);
                    this.node.RemoveEntry(positionCopy);
                }
                else
                {
                    childNode.Append(nextNode);
                    this.node.RemoveEntry(positionCopy + 1);
                }

                return;
            }

            public override void Dispose()
            {
                base.Dispose();
                this.childIterator = null;
                this.entries = null;
            }
        }

        #endregion
    }
}