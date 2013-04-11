namespace IndexedCollections
{
    using System;

    public abstract class BalancedTreeIterator<TKey, TValue> : IDisposable
    {
        #region Properties

        public abstract bool IsEnd { get; }
        
        public abstract bool IsDisposed { get; }

        public abstract TKey Key { get; }

        public abstract TValue Value { get; set; }

        #endregion

        #region Public Methods

        public abstract BalancedTreeIterator<TKey, TValue> Clone();

        public abstract bool MoveNext();

        public abstract void Remove();

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public abstract void Dispose();

        #endregion

        #endregion
    }
}