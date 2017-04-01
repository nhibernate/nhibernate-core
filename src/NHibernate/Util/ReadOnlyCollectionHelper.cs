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

    public static IReadOnlyList<T> ImmutableAdd<T>(this IReadOnlyList<T> collection, T value)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

			var result = new List<T>(collection.Count + 1);

			result.AddRange(collection);
			result.Add(value);
			
      return result.AsReadOnly();
    }

    public static IReadOnlyList<T> ImmutableAddRange<T>(this IReadOnlyList<T> collection, IList<T> values)
    {
      if (collection == null)
      {
        throw new ArgumentNullException("collection");
      }

      if (values == null)
      {
        throw new ArgumentNullException("values");
      }
			
			var result = new List<T>(collection.Count + values.Count);

			result.AddRange(collection);
			result.AddRange(values);
			
      return result.AsReadOnly();
    }
  }
}