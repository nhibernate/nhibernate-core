using System;
using System.Data.Common;

namespace NHibernate.AdoNet
{
	public class OracleDbDataReader : DbDataReaderWrapper
	{
		private readonly string _timestampFormat;

		public OracleDbDataReader(DbDataReader reader, string timestampFormat)
			: base(reader)
		{
			_timestampFormat = timestampFormat;
		}

		// Oracle driver does not implement GetChar
		public override char GetChar(int ordinal)
		{
			var value = DataReader[ordinal];

			return value switch
			{
				string { Length: > 0 } s => s[0],
				_ => (char) value
			};
		}

		public override DateTime GetDateTime(int ordinal)
		{
			var value = DataReader[ordinal];

			if (value is string && _timestampFormat != null)
			{
				return ParseDate((string)value);
			}

			return (DateTime) value;
		}

		private DateTime ParseDate(string value)
		{
			// Need to implment rules according to https://docs.oracle.com/en/database/oracle/oracle-database/19/sqlrf/Format-Models.html#GUID-49B32A81-0904-433E-B7FE-51606672183A
			throw new NotImplementedException($"Should parse '{value}' using '{_timestampFormat}'");
		}
	}
}
