using System;
using System.Collections;

using NHibernate.Dialect;

using NUnit.Framework;

namespace NHibernate.Test.GeneratedTest
{
	[TestFixture]
	public class TriggerGeneratedValuesWithCachingTest : AbstractGeneratedPropertyTest
	{
		protected override IList Mappings
		{
			get { return new string[] { "GeneratedTest.GeneratedPropertyEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect || dialect is Oracle8iDialect;
		}
	}
}
