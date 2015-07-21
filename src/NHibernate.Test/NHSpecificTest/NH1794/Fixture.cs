using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1794
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanQueryOnCollectionThatAppearsOnlyInTheMapping()
		{
			using (ISession session = OpenSession())
			{
				session
					.CreateQuery("select p.Name, c.Name from Person p join p.Children c")
					.List();
			}
		}

		[Test]
		public void CanQueryOnPropertyThatOnlyShowsUpInMapping_AsAccessNone()
		{
			using (ISession session = OpenSession())
			{
				session
					.CreateQuery("from Person p where p.UpdatedAt is null")
					.List();
			}
		}
	}
}