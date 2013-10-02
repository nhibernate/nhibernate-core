using System.Collections.Generic;
using System.Threading;

namespace NHibernate.AdoNet
{
	/// <summary> Implementation of ColumnNameCache. </summary>
	public class ColumnNameCache
	{
		private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
		private readonly Dictionary<string, int?> columnNameToIndexCache;

		public ColumnNameCache(int columnCount)
		{
			// should *not* need to grow beyond the size of the total number of columns in the rs
			columnNameToIndexCache = new Dictionary<string, int?>(columnCount);
		}

		public int GetIndexForColumnName(string columnName, ResultSetWrapper rs)
		{
			int? cached = Read(columnName);
			if (cached.HasValue)
			{
				return cached.Value;
			}
			else
			{
				int index = rs.Target.GetOrdinal(columnName);
				Insert(columnName, index);
				return index;
			}
		}

		private int? Read(string key)
		{
			cacheLock.EnterReadLock();
			try
			{
				int? value;
				columnNameToIndexCache.TryGetValue(key, out value);
				return value;
			}
			finally
			{
				cacheLock.ExitReadLock();
			}
		}

		private void Insert(string key, int value)
		{
			cacheLock.EnterWriteLock();
			try
			{
				columnNameToIndexCache[key] = value;
			}
			finally
			{
				cacheLock.ExitWriteLock();
			}
		}
	}
}