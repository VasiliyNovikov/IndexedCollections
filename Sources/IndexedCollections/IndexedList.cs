namespace IndexedCollections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public class IndexedList<T> : Collection<T>, IQueryable<T>
    {
        #region Properties

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}