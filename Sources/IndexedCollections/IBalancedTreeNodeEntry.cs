namespace IndexedCollections
{
    public interface IBalancedTreeNodeEntry<out TKey>
    {
        #region Properties

        TKey Key { get; }

        #endregion
    }
}