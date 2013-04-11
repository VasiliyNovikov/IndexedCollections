namespace IndexedCollections.Refactor
{
    internal class InnerNode<TKey, TValue> : Node<TKey, TValue, InnerEntry<TKey, TValue>>
    {
        public InnerNode(int capacity)
            : base(capacity)
        {
        }
    }
}