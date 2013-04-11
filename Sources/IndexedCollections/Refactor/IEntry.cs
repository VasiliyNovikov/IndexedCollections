namespace IndexedCollections.Refactor
{
    internal interface IEntry<TKey, TValue, TEntry>
        where TEntry : struct, IEntry<TKey, TValue, TEntry>
    {

    }
}