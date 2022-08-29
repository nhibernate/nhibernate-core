using System;
using System.Text;
using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.HqlOnMapWithForumula
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Mapping uses a scalar sub-select formula.
			return Dialect.SupportsScalarSubSelects;
		}

		[Test]
		public void TestBug()
		{
			using (ISession s = Sfi.OpenSession())
			{
				s.CreateQuery("from A a where 1 in elements(a.MyMaps)")
					.List();
			}
		}
	}
}
