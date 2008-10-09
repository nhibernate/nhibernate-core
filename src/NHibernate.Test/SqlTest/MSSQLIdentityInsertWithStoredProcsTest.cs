using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class MSSQLIdentityInsertWithStoredProcsTest : IdentityInsertWithStoredProcsTest
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect || dialect is MsSql2005Dialect;
		}

		protected override string GetExpectedInsertOrgLogStatement(string orgName)
		{
			return string.Format("exec nh_organization_native_id_insert @p0; @p0 = '{0}'", orgName);
		}

		protected override IList Mappings
		{
			get { return new[] {"SqlTest.MSSQLIdentityInsertWithStoredProcs.hbm.xml"}; }
		}
	}
}