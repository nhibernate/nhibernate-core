using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.AdoNet
{
	/// <summary>
	/// Many database drivers lack support for DbDataReader.GetChar and throw a
	/// NotSupportedException. This reader provides an implementation on top of
	/// the indexer method for defficient drivers.
	/// </summary>
	public class NoCharDbDataReader : DbDataReaderWrapper
	{
		public NoCharDbDataReader(DbDataReader reader) : base(reader) { }

		public override char GetChar(int ordinal)
		{
			// The underlying DataReader does not support the GetChar method.
			// Use the indexer to obtain the value and convert it to a char if necessary.
			var value = DataReader[ordinal];

			return value switch
			{
				string { Length: > 0 } s => s[0],
				_ => (char) value
			};
		}
	}
}
