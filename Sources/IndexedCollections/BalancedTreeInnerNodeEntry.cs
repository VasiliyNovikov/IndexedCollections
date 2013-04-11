namespace IndexedCollections
{
    public struct BalancedTreeInnerNodeEntry<TKey, TValue> : IBalancedTreeNodeEntry<TKey>
    {
        #region Constants and Fields

        public readonly BalancedTreeNode<TKey, TValue> ChildNode;

        public TKey Key;

        #endregion

        #region Constructors and Destructors

        public BalancedTreeInnerNodeEntry(BalancedTreeNode<TKey, TValue> childNode)
            : this()
        {
            this.Key = childNode.FirstKey;
            this.ChildNode = childNode;
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