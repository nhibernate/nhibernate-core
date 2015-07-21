using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2441
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return ((dialect is Dialect.SQLiteDialect) || (dialect is Dialect.MsSql2008Dialect));
		}
		
		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person e1 = new Person("Tuna Toksoz", "Born in Istanbul :Turkey");
				Person e2 = new Person("Tuna Toksoz", "Born in Istanbul :Turkiye");
				s.Save(e1);
				s.Save(e2);
				tx.Commit();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				tx.Commit();
			}
			
			base.OnTearDown();
		}

		[Test]
		public void LinqQueryBooleanSQLite()
		{
			using (ISession session = OpenSession())
			{
				var query1 = session.Query<Person>().Where(p => true);
				var query2 = session.Query<Person>().Where(p => p.Id != null);
				var query3 = session.Query<Person>();
				
				Assert.That(query1.Count(), Is.EqualTo(query2.Count()));
				Assert.That(query3.Count(), Is.EqualTo(query1.Count()));
			}
		}
	}
}
