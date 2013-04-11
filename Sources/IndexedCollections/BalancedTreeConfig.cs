namespace IndexedCollections
{
    using System;
    using System.Collections.Generic;

    public class BalancedTreeConfig<TKey>
    {
        public readonly int Capacity;

        public readonly bool IsReversed;

        public readonly IComparer<TKey> KeyComparer;

        public readonly int MinNodeSize;

        public BalancedTreeConfig(int minNodeSize, bool isReversed)
        {
            if (minNodeSize < 1)
            {
                throw new ArgumentOutOfRangeException("minNodeSize");
            }

            this.MinNodeSize = minNodeSize;
            this.IsReversed = isReversed;
            this.Capacity = minNodeSize << 1;
            this.KeyComparer = isReversed ? (IComparer<TKey>)ReversedComparer<TKey>.Default : Comparer<TKey>.Default;
        }
    }
}