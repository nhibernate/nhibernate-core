using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1447
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var e1 = new Person("Tuna Toksoz",false);
					var e2 = new Person("Oguz Kurumlu", true);
					s.Save(e1);
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanQueryByConstantProjectionWithType()
		{
			using (ISession s = OpenSession())
			{
				ICriteria c = s.CreateCriteria(typeof (Person))
					.Add(Restrictions.EqProperty("WantsNewsletter", Projections.Constant(false,NHibernateUtil.Boolean)));
				IList<Person> list = c.List<Person>();
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}
