using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.Oracle
{
	[TestFixture, Ignore("Not supported yet.")]
	public class OracleCustomSQLFixture : CustomStoredProcSupportTest
	{
		protected override IList Mappings
		{
			get { return new[] { "SqlTest.Custom.Oracle.Mappings.hbm.xml", "SqlTest.Custom.Oracle.StoredProcedures.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Oracle8iDialect;
		}
	}
}