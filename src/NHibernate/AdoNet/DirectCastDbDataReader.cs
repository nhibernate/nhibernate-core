using System;
using System.Collections;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace NHibernate.AdoNet
{
	public partial class DirectCastDbDataReader : DbDataReaderWrapper
	{
		public DirectCastDbDataReader(DbDataReader dbDataReader) : base(dbDataReader) { }

		public override object this[int ordinal] => DataReader[ordinal];

		public override object this[string name] => DataReader[name];

		public override int Depth => DataReader.Depth;

		public override int FieldCount => DataReader.FieldCount;

		public override bool HasRows => DataReader.HasRows;

		public override bool IsClosed => DataReader.IsClosed;

		public override int RecordsAffected => DataReader.RecordsAffected;

		public override bool GetBoolean(int ordinal)
		{
			return (bool) DataReader[ordinal];
		}

		public override byte GetByte(int ordinal)
		{
			return (byte) DataReader[ordinal];
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			return DataReader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
		}

		public override char GetChar(int ordinal)
		{
			return (char) DataReader[ordinal];
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
			return (DateTime) DataReader[ordinal];
		}

		public override decimal GetDecimal(int ordinal)
		{
			return (decimal) DataReader[ordinal];
		}

		public override double GetDouble(int ordinal)
		{
			return (double) DataReader[ordinal];
		}

		public override IEnumerator GetEnumerator()
		{
			while (DataReader.Read())
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
			return (float) DataReader[ordinal];
		}

		public override Guid GetGuid(int ordinal)
		{
			return (Guid) DataReader[ordinal];
		}

		public override short GetInt16(int ordinal)
		{
			return (short) DataReader[ordinal];
		}

		public override int GetInt32(int ordinal)
		{
			return (int) DataReader[ordinal];
		}

		public override long GetInt64(int ordinal)
		{
			return (long) DataReader[ordinal];
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
			return (string) DataReader[ordinal];
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
