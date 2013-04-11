namespace IndexedCollections
{
    public struct BalancedTreeLeafNodeEntry<TKey, TValue> : IBalancedTreeNodeEntry<TKey>
    {
        #region Constants and Fields

        public readonly TKey Key;

        public TValue Value;

        #endregion

        #region Constructors and Destructors

        public BalancedTreeLeafNodeEntry(TKey key, TValue value)
            : this()
        {
            this.Key = key;
            this.Value = value;
        }

        #endregion

        #region Properties

        TKey IBalancedTreeNodeEntry<TKey>.Key
        {
            get

            {
                return this.Key;
            }
        }

        #endregion
    }
}