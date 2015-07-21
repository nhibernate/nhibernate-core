using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.MsSQL
{
	[TestFixture]
	public class MSSQLTest : CustomStoredProcSupportTest
	{
		protected override IList Mappings
		{
			get { return new[] { "SqlTest.Custom.MsSQL.MSSQLEmployment.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}
	}
}