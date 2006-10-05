using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class FireBirdTest : HandSQLTest
	{
		protected override IList Mappings
		{
			get { return new string[] { "SqlTest.FireBirdEmployment.hbm.xml" }; }
		}

		protected override System.Type GetDialect()
		{
			return typeof(NHibernate.Dialect.FirebirdDialect);
		}
	}
}
