using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NHibernate.Engine.Query.Sql;


namespace NHibernate.Util
{
  public static class ReadOnlyCollectionHelper
  {
    internal static readonly ReadOnlyCollection<INativeSQLQueryReturn> EmptyQueryReturns =
      new ReadOnlyCollection<INativeSQLQueryReturn>(new INativeSQLQueryReturn[0]);

    public static ReadOnlyCollection<T> ImmutableAdd<T>(this ReadOnlyCollection<T> collection, T value)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

      T[] tmp = new T[collection.Count + 1];

      collection.CopyTo(tmp, 0);

      tmp[tmp.Length - 1] = value;

      return new ReadOnlyCollection<T>(tmp);
    }

    public static ReadOnlyCollection<T> ImmutableAddRange<T>(this ReadOnlyCollection<T> collection, IList<T> values)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

      if (values == null)
      {
        throw new ArgumentNullException("values");
      }

      T[] tmp = new T[collection.Count + values.Count];

      collection.CopyTo(tmp, 0);
      values.CopyTo(tmp, collection.Count);
      
      return new ReadOnlyCollection<T>(tmp);
    }
  }
}