using System.Collections.Generic;

namespace NHibernate.AdoNet
{
	/// <summary> Implementation of ColumnNameCache. </summary>
	public class ColumnNameCache
	{
		private readonly Dictionary<string, int?> columnNameToIndexCache;

		public ColumnNameCache(int columnCount)
		{
			// should *not* need to grow beyond the size of the total number of columns in the rs
			columnNameToIndexCache = new Dictionary<string, int?>(columnCount);
		}

		public int GetIndexForColumnName(string columnName, ResultSetWrapper rs)
		{
			int? cached;
			columnNameToIndexCache.TryGetValue(columnName, out cached);
			if (cached.HasValue)
			{
				return cached.Value;
			}
			else
			{
				int index = rs.Target.GetOrdinal(columnName);
				columnNameToIndexCache[columnName] = index;
				return index;
			}
		}
	}
}