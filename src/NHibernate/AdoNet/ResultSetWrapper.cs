using System;
using System.Collections;
using System.Data;
using System.Data.Common;
#if ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif

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
		private DbDataReader rs;
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

		public override void Close()
		{
			rs.Close();
		}

		public override DataTable GetSchemaTable()
		{
			return rs.GetSchemaTable();
		}

		public override bool NextResult()
		{
			return rs.NextResult();
		}

		public override bool Read()
		{
			return rs.Read();
		}

#if ASYNC
		public override Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			return rs.ReadAsync(cancellationToken);
		}

		public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return rs.NextResultAsync(cancellationToken);
		}

		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return rs.IsDBNullAsync(ordinal, cancellationToken);
		}
#endif

		public override int Depth
		{
			get { return rs.Depth; }
		}

		public override bool HasRows
		{
			get { return rs.HasRows; }
		}

		public override bool IsClosed
		{
			get { return rs.IsClosed; }
		}

		public override int RecordsAffected
		{
			get { return rs.RecordsAffected; }
		}

		private bool disposed;

		protected override void Dispose(bool disposing)
		{
			if (disposed)
				return;

			if (disposing && rs != null)
			{
					rs.Dispose();
				rs = null;
				}

			disposed = true;
		}

		public override string GetName(int i)
		{
			return rs.GetName(i);
		}

		public override string GetDataTypeName(int i)
		{
			return rs.GetDataTypeName(i);
		}

		public override IEnumerator GetEnumerator()
		{
			return rs.GetEnumerator();
		}

		public override System.Type GetFieldType(int i)
		{
			return rs.GetFieldType(i);
		}

		public override object GetValue(int i)
		{
			return rs.GetValue(i);
		}

		public override int GetValues(object[] values)
		{
			return rs.GetValues(values);
		}

		public override int GetOrdinal(string name)
		{
			return columnNameCache.GetIndexForColumnName(name, this);
		}

		public override bool GetBoolean(int i)
		{
			return rs.GetBoolean(i);
		}

		public override byte GetByte(int i)
		{
			return rs.GetByte(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return rs.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public override char GetChar(int i)
		{
			return rs.GetChar(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return rs.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public override Guid GetGuid(int i)
		{
			return rs.GetGuid(i);
		}

		public override short GetInt16(int i)
		{
			return rs.GetInt16(i);
		}

		public override int GetInt32(int i)
		{
			return rs.GetInt32(i);
		}

		public override long GetInt64(int i)
		{
			return rs.GetInt64(i);
		}

		public override float GetFloat(int i)
		{
			return rs.GetFloat(i);
		}

		public override double GetDouble(int i)
		{
			return rs.GetDouble(i);
		}

		public override string GetString(int i)
		{
			return rs.GetString(i);
		}

		public override decimal GetDecimal(int i)
		{
			return rs.GetDecimal(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return rs.GetDateTime(i);
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			return rs.GetData(ordinal);
		}

		public override bool IsDBNull(int i)
		{
			return rs.IsDBNull(i);
		}

		public override int FieldCount
		{
			get { return rs.FieldCount; }
		}

		public override object this[int i]
		{
			get { return rs[i]; }
		}

		public override object this[string name]
		{
			get { return rs[columnNameCache.GetIndexForColumnName(name, this)]; }
		}

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
