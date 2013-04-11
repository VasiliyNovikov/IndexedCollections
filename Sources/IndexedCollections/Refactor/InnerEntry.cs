namespace IndexedCollections.Refactor
{
    struct InnerEntry<TKey, TValue> : IEntry<TKey, TValue, InnerEntry<TKey, TValue>>
    {
        public readonly TKey Key;

        public readonly Node<TKey, TValue> Child;

        public InnerEntry(TKey key, Node<TKey, TValue> child)
            : this()
        {
            this.Key = key;
            this.Child = child;
        }
    }
}
