namespace IndexedCollections.Refactor
{
    internal class Node<TKey, TValue>
    {
        
    }

    internal class Node<TKey, TValue, TEntry> : Node<TKey, TValue>
        where TEntry : struct, IEntry<TKey, TValue, TEntry>
    {
        public readonly TEntry[] Entries;

        public Node(int capacity)
        {
            this.Entries = new TEntry[capacity];
        }
    }
}