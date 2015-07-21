using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1868
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					cat = new Category {ValidUntil = DateTime.Now};
					session.Save(cat);

					package = new Package {ValidUntil = DateTime.Now};
					session.Save(package);

					tx.Commit();
				}
			}
		}

		private Category cat;
		private Package package;

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Category");
					session.Delete("from Package");
					tx.Commit();
				}
			}
			base.OnTearDown();
		}

		public void ExecuteQuery(Action<ISession> sessionModifier)
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					sessionModifier(session);
					session.Refresh(cat);
					session.Refresh(package);

					session.CreateQuery(
						@"
                    select 
                        inv
                    from 
                        Invoice inv
                        , Package p
                    where
                        p = :package
                        and inv.Category = :cat
                        and inv.ValidUntil > :now
                        and inv.Package = :package 
                    ")
						.SetEntity("cat", cat).SetEntity("package", package).SetDateTime("now", DateTime.Now).UniqueResult<Invoice>();

					tx.Commit();
				}
			}
		}

		[Test]
		public void Bug()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}

		[Test]
		public void FilterOnOffOn()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => { }));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}

		[Test]
		public void FilterQueryTwice()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}

		[Test]
		public void FilterQuery3()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}
	}
}
