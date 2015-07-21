using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Futures
{
	public class LinqToFutureValueFixture : FutureFixture
	{
		[Test]
		public void CanExecuteToFutureValueCount()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var personsCount = session.Query<Person>()
					.Where(x => x.Name == "Test1")
					.ToFutureValue(x => x.Count());

				Assert.AreEqual(1, personsCount.Value);
			}
		}

		[Test]
		public void CanExecuteToFutureValueCountWithPredicate()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var personsCount = session.Query<Person>()
					.ToFutureValue(q => q.Count(x => x.Name == "Test1"));

				Assert.AreEqual(1, personsCount.Value);
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Person {Name = "Test1"});
				session.Save(new Person {Name = "Test2"});
				session.Save(new Person {Name = "Test3"});
				session.Save(new Person {Name = "Test4"});
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}
	}
}