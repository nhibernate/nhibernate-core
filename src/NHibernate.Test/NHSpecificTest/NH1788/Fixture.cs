using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1788
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void CanUseSqlTimestampWithDynamicInsert()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new Person
				{
					Name = "hi"
				});
				tx.Commit();
			}


			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var person = session.Get<Person>(1);
				person.Name = "other";
				tx.Commit();
			} 


			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete(session.Get<Person>(1));
				tx.Commit();
			}
			
		}
	}
}