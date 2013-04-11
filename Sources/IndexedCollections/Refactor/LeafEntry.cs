namespace IndexedCollections.Refactor
{
    internal struct LeafEntry<TKey, TValue> : IEntry<TKey, TValue, LeafEntry<TKey, TValue>>
    {
        #region Fields

        public readonly TKey Key;

        public TValue Value;

        #endregion

        public LeafEntry(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}