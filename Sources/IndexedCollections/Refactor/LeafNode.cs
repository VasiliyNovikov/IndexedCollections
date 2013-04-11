namespace IndexedCollections.Refactor
{
    internal class LeafNode<TKey, TValue> : Node<TKey, TValue, LeafEntry<TKey, TValue>>
    {
        public LeafNode(int capacity)
            : base(capacity)
        {
        }
    }
}