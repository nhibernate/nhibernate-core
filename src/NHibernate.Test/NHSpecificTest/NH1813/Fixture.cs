using NHibernate.Cfg;
using NHibernate.Exceptions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1813
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchSize, "0");
		}

		[Test]
		public void ContainSQLInInsert()
		{
			using (ISession s = OpenSession())
			using(ITransaction t = s .BeginTransaction())
			{
				s.Save(new EntityWithUnique {Id = 1, Description = "algo"});
				t.Commit();
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new EntityWithUnique { Id = 2, Description = "algo" });
				var exception = Assert.Throws<GenericADOException>(t.Commit);
				Assert.That(exception.Message, Is.StringContaining("INSERT"), "should contain SQL");
				Assert.That(exception.Message, Is.StringContaining("#2"), "should contain id");
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityWithUnique").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void ContainSQLInUpdate()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new EntityWithUnique { Id = 1, Description = "algo" });
				s.Save(new EntityWithUnique { Id = 2, Description = "mas" });
				t.Commit();
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var e = s.Get<EntityWithUnique>(2);
				e.Description = "algo";
				var exception = Assert.Throws<GenericADOException>(t.Commit);
				Assert.That(exception.Message, Is.StringContaining("UPDATE"), "should contain SQL");
			}
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from EntityWithUnique").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}