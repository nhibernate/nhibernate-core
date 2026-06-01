using System;
using System.Data.Common;
using System.Globalization;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// This is a wrapper for the DbDataReader returned by the SqlAnywhere driver.
	/// The DbDataReader in the SqlAnywhere driver does not support the GetChar method,
	/// and uses Convert.To* without specifying an <see cref="IFormatProvider"/>.
	/// </summary>
	public class SqlAnywhereDbDataReader : NoCharDbDataReader
	{
		public SqlAnywhereDbDataReader(DbDataReader reader) : base(reader) { }

		public override float GetFloat(int ordinal)
		{
			var value = DataReader[ordinal];

			return value switch
			{
				string s => float.Parse(s, CultureInfo.InvariantCulture),
				_ => (float) value
			};
		}

		public override double GetDouble(int ordinal)
		{
			var value = DataReader[ordinal];

			return value switch
			{
				string s => double.Parse(s, CultureInfo.InvariantCulture),
				_ => (double) value
			};
		}

		public override decimal GetDecimal(int ordinal)
		{
			var value = DataReader[ordinal];

			return value switch
			{
				string s => decimal.Parse(s, CultureInfo.InvariantCulture),
				_ => (decimal) value
			};
		}

		public override DateTime GetDateTime(int ordinal)
		{
			var value = DataReader[ordinal];

			return value switch
			{
				string s => DateTime.Parse(s, CultureInfo.InvariantCulture),
				_ => (DateTime) value
			};
		}
	}
}
