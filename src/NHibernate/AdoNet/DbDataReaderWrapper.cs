using System;
using System.Collections;
using System.Data.Common;

namespace NHibernate.AdoNet
{
	public abstract partial class DbDataReaderWrapper : DbDataReader
	{
		protected DbDataReader DataReader { get; private set; }

		public override int Depth => DataReader.Depth;

		public override int FieldCount => DataReader.FieldCount;

		public override bool HasRows => DataReader.HasRows;

		public override bool IsClosed => DataReader.IsClosed;

		public override int RecordsAffected => DataReader.RecordsAffected;

		public override object this[string name] => DataReader[name];

		public override object this[int ordinal] => DataReader[ordinal];

		public DbDataReaderWrapper(DbDataReader dbDataReader)
		{
			DataReader = dbDataReader;
		}

		public override bool GetBoolean(int ordinal)
		{
			return DataReader.GetBoolean(ordinal);
		}

		public override byte GetByte(int ordinal)
		{
			return DataReader.GetByte(ordinal);
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			return DataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
		}

		public override char GetChar(int ordinal)
		{
			return DataReader.GetChar(ordinal);
		}

		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			return DataReader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
		}

		public override string GetDataTypeName(int ordinal)
		{
			return DataReader.GetDataTypeName(ordinal);
		}

		public override DateTime GetDateTime(int ordinal)
		{
			return DataReader.GetDateTime(ordinal);
		}

		public override decimal GetDecimal(int ordinal)
		{
			return DataReader.GetDecimal(ordinal);
		}

		public override double GetDouble(int ordinal)
		{
			return DataReader.GetDouble(ordinal);
		}

		public override IEnumerator GetEnumerator()
		{
			while (Read())
			{
				yield return this;
			}
		}

		public override System.Type GetFieldType(int ordinal)
		{
			return DataReader.GetFieldType(ordinal);
		}

		public override float GetFloat(int ordinal)
		{
			return DataReader.GetFloat(ordinal);
		}

		public override Guid GetGuid(int ordinal)
		{
			return DataReader.GetGuid(ordinal);
		}

		public override short GetInt16(int ordinal)
		{
			return DataReader.GetInt16(ordinal);
		}

		public override int GetInt32(int ordinal)
		{
			return DataReader.GetInt32(ordinal);
		}

		public override long GetInt64(int ordinal)
		{
			return DataReader.GetInt64(ordinal);
		}

		public override string GetName(int ordinal)
		{
			return DataReader.GetName(ordinal);
		}

		public override int GetOrdinal(string name)
		{
			return DataReader.GetOrdinal(name);
		}

		public override string GetString(int ordinal)
		{
			return DataReader.GetString(ordinal);
		}

		public override object GetValue(int ordinal)
		{
			return DataReader.GetValue(ordinal);
		}

		public override int GetValues(object[] values)
		{
			return DataReader.GetValues(values);
		}

		public override bool IsDBNull(int ordinal)
		{
			return DataReader.IsDBNull(ordinal);
		}

		public override bool NextResult()
		{
			return DataReader.NextResult();
		}

		public override bool Read()
		{
			return DataReader.Read();
		}

		public override void Close()
		{
			DataReader.Close();
		}
	}
}
