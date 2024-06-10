using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.AdoNet
{
	public class MySqlDbDataReader : DbDataReaderWrapper
	{
		public MySqlDbDataReader(DbDataReader reader) : base(reader) { }

		// MySql driver has a bug that incorrectly uses the CurrentCulture to parse strings.
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
	}
}
