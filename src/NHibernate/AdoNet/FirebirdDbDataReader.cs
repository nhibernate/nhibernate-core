using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Id.Insert;

namespace NHibernate.AdoNet
{
	public class FirebirdDbDataReader : DbDataReaderWrapper
	{
		public FirebirdDbDataReader(DbDataReader reader) : base(reader) { }

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
