using System;
using System.Collections;

using NHibernate.Dialect;

using NUnit.Framework;

namespace NHibernate.Test.GeneratedTest
{
	[TestFixture]
	public class TimestampGeneratedValuesWithCachingTest : AbstractGeneratedPropertyTest
	{
		protected override IList Mappings
		{
			get { return new string[] { "GeneratedTest.MSSQLGeneratedPropertyEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// this test is specific to SQL Server as it is testing support
			// for its TIMESTAMP datatype...
			return dialect is MsSql2000Dialect;
		}
	}
}
