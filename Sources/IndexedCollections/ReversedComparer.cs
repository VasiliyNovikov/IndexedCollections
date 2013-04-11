namespace IndexedCollections
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares in the order reversed to orignal
    /// </summary>
    public class ReversedComparer<T> : IComparer<T>
    {
        #region Constants and Fields

        /// <summary>
        /// Original comparer
        /// </summary>
        private readonly IComparer<T> comparer;

        #endregion

        #region Constructors and Destructors

        static ReversedComparer()
        {
            Default = new ReversedComparer<T>(Comparer<T>.Default);
        }

        public ReversedComparer(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        #endregion

        #region Properties

        public static ReversedComparer<T> Default { get; private set; }

        #endregion

        #region Implemented Interfaces

        #region IComparer<T>

        public int Compare(T x, T y)
        {
            return this.comparer.Compare(y, x);
        }

        #endregion

        #endregion
    }
}