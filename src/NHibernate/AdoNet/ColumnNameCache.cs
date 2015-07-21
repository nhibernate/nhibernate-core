using System.Collections.Generic;
using System.Threading;

namespace NHibernate.AdoNet
{
	/// <summary> Implementation of ColumnNameCache. Thread safe. </summary>
	public class ColumnNameCache
	{
		private readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
		private readonly Dictionary<string, int> _columnNameToIndexCache;

		public ColumnNameCache(int columnCount)
		{
			// should *not* need to grow beyond the size of the total number of columns in the rs
			_columnNameToIndexCache = new Dictionary<string, int>(columnCount);
		}

		public int GetIndexForColumnName(string columnName, ResultSetWrapper rs)
		{
			int index;
			if (!TryRead(columnName, out index))
			{
				index = rs.Target.GetOrdinal(columnName);
				Insert(columnName, index);
			}

			return index;
		}

		private bool TryRead(string key, out int value)
		{
			_cacheLock.EnterReadLock();
			try
			{
				return _columnNameToIndexCache.TryGetValue(key, out value);
			}
			finally
			{
				_cacheLock.ExitReadLock();
			}
		}

		private void Insert(string key, int value)
		{
			_cacheLock.EnterWriteLock();
			try
			{
				_columnNameToIndexCache[key] = value;
			}
			finally
			{
				_cacheLock.ExitWriteLock();
			}
		}
	}
}