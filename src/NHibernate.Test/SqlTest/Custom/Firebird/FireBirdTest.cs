using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Custom.Firebird
{
	[TestFixture]
	public class FireBirdTest : CustomStoredProcSupportTest
	{
		protected override IList Mappings
		{
			get { return new[] {"SqlTest.Custom.Firebird.FireBirdEmployment.hbm.xml"}; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is FirebirdDialect;
		}
	}
}