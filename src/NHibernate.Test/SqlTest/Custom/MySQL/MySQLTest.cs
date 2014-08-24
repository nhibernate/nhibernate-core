using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.MySQL
{
	[TestFixture]
	public class MySQLTest : CustomStoredProcSupportTest
	{
		protected override IList Mappings
		{
			get { return new[] { "SqlTest.Custom.MySQL.MySQLEmployment.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MySQL5Dialect || dialect is MySQLDialect;
		}
	}
}