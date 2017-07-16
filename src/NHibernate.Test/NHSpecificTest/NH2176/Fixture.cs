using System;
using System.Transactions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2176
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var steve = new Person { Name = "Steve" };
				var peter = new Person { Name = "Peter" };
				var simon = new Person { Name = "Simon" };
				var paul = new Person { Name = "Paul" };
				var john = new Person { Name = "John" };
				var eric = new Person { Name = "Eric" };

				s.Save(steve);
				s.Save(peter);
				s.Save(simon);
				s.Save(paul);
				s.Save(john);
				s.Save(eric);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		// Whilst this bug seems specific to Oracle I think it is valid to run the
		// test against all database types.
		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support distributed transactions.")]
#endif
		public void MultipleConsecutiveTransactionScopesCanBeUsedInsideASingleSession()
		{
			using (var s = OpenSession())
			{
				// usually fails after just a few loops in oracle
				// this can be run for 10000 loops in sql server without problem
				for (var i = 0; i < 100; ++i)
				{
					Console.WriteLine(i.ToString());

					using (var scope = new TransactionScope())
					{
						var criteria = s.CreateCriteria<Person>();
						var people = criteria.List<Person>();

						Assert.That(people.Count, Is.EqualTo(6));

						scope.Complete();
					}

					// The exception is caused by a race condition between two threads.
					// This can be demonstrated by uncommenting the following line which
					// causes the test to run without an exception.
					//System.Threading.Thread.Sleep(1000);
				}
			}
		}
	}
}
