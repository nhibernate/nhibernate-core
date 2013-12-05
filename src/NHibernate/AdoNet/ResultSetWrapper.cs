using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.AdoNet
{
	/// <summary> 
	/// A ResultSet delegate, responsible for locally caching the columnName-to-columnIndex
	/// resolution that has been found to be inefficient in a few vendor's drivers (i.e., Oracle
	/// and Postgres). 
	/// </summary>
	/// <seealso cref="IDataRecord.GetOrdinal"/>
	public class ResultSetWrapper : DbDataReader
	{
		private readonly DbDataReader rs;
		private readonly ColumnNameCache columnNameCache;

		public ResultSetWrapper(DbDataReader resultSet, ColumnNameCache columnNameCache)
		{
			rs = resultSet;
			this.columnNameCache = columnNameCache;
		}

		internal DbDataReader Target
		{
			get { return rs; }
		}

		#region DbDataReader Members

		public void Close()
		{
			rs.Close();
		}

		public DataTable GetSchemaTable()
		{
			return rs.GetSchemaTable();
		}

		public bool NextResult()
		{
			return rs.NextResult();
		}

		public bool Read()
		{
			return rs.Read();
		}

		public int Depth
		{
			get { return rs.Depth; }
		}

		public bool IsClosed
		{
			get { return rs.IsClosed; }
		}

		public int RecordsAffected
		{
			get { return rs.RecordsAffected; }
		}

		#endregion

		#region IDisposable Members
		private bool disposed;

		~ResultSetWrapper()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing)
			{
				if (rs != null)
				{
					if (!rs.IsClosed) rs.Close();
					rs.Dispose();
				}
			}

			disposed = true;
		}
		#endregion

		#region IDataRecord Members

		public string GetName(int i)
		{
			return rs.GetName(i);
		}

		public string GetDataTypeName(int i)
		{
			return rs.GetDataTypeName(i);
		}

		public System.Type GetFieldType(int i)
		{
			return rs.GetFieldType(i);
		}

		public object GetValue(int i)
		{
			return rs.GetValue(i);
		}

		public int GetValues(object[] values)
		{
			return rs.GetValues(values);
		}

		public int GetOrdinal(string name)
		{
			return columnNameCache.GetIndexForColumnName(name, this);
		}

		public bool GetBoolean(int i)
		{
			return rs.GetBoolean(i);
		}

		public byte GetByte(int i)
		{
			return rs.GetByte(i);
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return rs.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public char GetChar(int i)
		{
			return rs.GetChar(i);
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return rs.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public Guid GetGuid(int i)
		{
			return rs.GetGuid(i);
		}

		public short GetInt16(int i)
		{
			return rs.GetInt16(i);
		}

		public int GetInt32(int i)
		{
			return rs.GetInt32(i);
		}

		public long GetInt64(int i)
		{
			return rs.GetInt64(i);
		}

		public float GetFloat(int i)
		{
			return rs.GetFloat(i);
		}

		public double GetDouble(int i)
		{
			return rs.GetDouble(i);
		}

		public string GetString(int i)
		{
			return rs.GetString(i);
		}

		public decimal GetDecimal(int i)
		{
			return rs.GetDecimal(i);
		}

		public DateTime GetDateTime(int i)
		{
			return rs.GetDateTime(i);
		}

		public DbDataReader GetData(int i)
		{
			return rs.GetData(i);
		}

		public bool IsDBNull(int i)
		{
			return rs.IsDBNull(i);
		}

		public int FieldCount
		{
			get { return rs.FieldCount; }
		}

		public object this[int i]
		{
			get { return rs[i]; }
		}

		public object this[string name]
		{
			get { return rs[columnNameCache.GetIndexForColumnName(name, this)]; }
		}

		#endregion

		public override bool Equals(object obj)
		{
			return rs.Equals(obj);
		}

		public override int GetHashCode()
		{
			return rs.GetHashCode();
		}
	}
}
