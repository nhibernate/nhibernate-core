using System;

using NUnit.Framework;

namespace NHibernate.Test.DialectTest 
{

	/// <summary>
	/// Summary description for MsSqlDialectFixture.
	/// </summary>
	[TestFixture]
	public class MsSqlDialectFixture : DialectFixture
	{

		[SetUp]
		public override void SetUp() 
		{
			// Generic Dialect inherits all of the Quoting functions from
			// Dialect (which is abstract)
			d = new Dialect.MsSql2000Dialect();
			tableWithNothingToBeQuoted = new string[] {"plainname", "[plainname]"};
			tableAlreadyQuoted = new string[] {"[Quote[d[Na]]$`]", "[Quote[d[Na]]$`]","Quote[d[Na]$`" };
			tableThatNeedsToBeQuoted = new string[] {"Quote[d[Na]$`", "[Quote[d[Na]]$`]", "Quote[d[Na]$`"};

		}
	}
}
