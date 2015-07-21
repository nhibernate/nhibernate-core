using System;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2224
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is NHibernate.Dialect.SQLiteDialect;
		}
		
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var class1 = new Class1() { DateOfChange = DateTime.Now };
				s.Save(class1);
				t.Commit();				
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Class1");
				t.Commit();
			}
			base.OnTearDown();
		}

		[Test]
		public void CanQueryBasedOnYearWithInOperator()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var criteria = s.CreateCriteria<Class1>();
				criteria.Add(Restrictions.In(
					Projections.SqlFunction(
						"year",
						NHibernateUtil.DateTime,
						Projections.Property("DateOfChange")),
						new string[] { "2010", DateTime.Now.Year.ToString() }));

				var result = criteria.List();
				
				Assert.That(result.Count, Is.EqualTo(1));
			}
		}
	}
}