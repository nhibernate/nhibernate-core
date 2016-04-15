using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3506
{
	[TestFixture]
	public class DescriminatorFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
            {
                var e1 = new Person {Name = "Bob"};
				session.Save(e1);

                var e2 = new Person { Name = "Sally" };
                session.Save(e2);

                var e3 = new Employer { Name = "Company ABC" };
                session.Save(e3);

                e1.Employer = e3;

                session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void DescriminatorFilterIsInFromFragment()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
                // bug only occurs when a filter is enabled with use-many-to-one=true
                session.EnableFilter("deletedFilter");

                var result = session.QueryOver<Person>()
                    .JoinQueryOver(p => p.Employer, SqlCommand.JoinType.LeftOuterJoin);

				Assert.AreEqual(2, result.List().Count);
			}
		}
	}
}