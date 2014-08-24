using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1694
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		private void FillDb()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					var newUser = new User();
					var newOrder1 = new Orders {User = newUser, Status = true};
					var newOrder2 = new Orders {User = newUser, Status = true};

					session.Save(newUser);
					session.Save(newOrder1);
					session.Save(newOrder2);

					newUser = new User();
					newOrder1 = new Orders {User = newUser, Status = false};

					session.Save(newUser);
					session.Save(newOrder1);

					tran.Commit();
				}
			}
		}

		private void Cleanup()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					session.Delete("from Orders");
					session.Delete("from User");
					tran.Commit();
				}
			}
		}

		[Test]
		public void CanOrderByExpressionContainingACommaInAPagedQuery()
		{
			FillDb();
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					ICriteria crit = session.CreateCriteria(typeof (User));
					crit.AddOrder(Order.Desc("OrderStatus"));
					crit.AddOrder(Order.Asc("Id"));
					crit.SetMaxResults(10);

					IList<User> list = crit.List<User>();

					Assert.That(list.Count, Is.EqualTo(2));
					Assert.That(list[0].OrderStatus, Is.EqualTo(2));
					Assert.That(list[1].OrderStatus, Is.EqualTo(1));

					tran.Commit();
				}
			}
			Cleanup();
		}
	}

	public class User
	{
		public virtual int Id { get; set; }
		public virtual int OrderStatus { get; set; }
	}

	public class Orders
	{
		public virtual int Id { get; set; }
		public virtual bool Status { get; set; }
		public virtual User User { get; set; }
	}
}