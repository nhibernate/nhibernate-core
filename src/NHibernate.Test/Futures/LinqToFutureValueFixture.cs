using System;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	[TestFixture]
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

		[Test(Description = "https://github.com/nhibernate/nhibernate-core/issues/1387")]
		public void ToFutureValueWithSumReturnsResult()
		{
			using (var s = OpenSession())
			{
				var personsSum = s.Query<Person>()
					.Select(x => x.Id)
					.ToFutureValue(x => x.Sum());

				Assert.IsNotNull(personsSum);
				Assert.NotZero(personsSum.Value);
			}
		}

		[Test]
		public void ToFutureValueWithSumOnEmptySetThrows()
		{
			using (var s = OpenSession())
			{
				var personsSum = s.Query<Person>()
					.Where(x => false) // make this an empty set
					.Select(x => x.Id)
					.ToFutureValue(x => x.Sum());

				Assert.That(() => personsSum.Value, Throws.InnerException.TypeOf<InvalidOperationException>().Or.InnerException.TypeOf<ArgumentNullException>());
			}
		}

		[Test]
		public void ToFutureValueWithNullableSumReturnsResult()
		{
			using (var s = OpenSession())
			{
				var ageSum = s.Query<Person>()
					.Select(x => x.Age)
					.ToFutureValue(x => x.Sum());

				Assert.IsNotNull(ageSum);
				Assert.IsNotNull(ageSum.Value);
				Assert.NotZero(ageSum.Value.Value);
			}
		}

		[Test]
		public void ToFutureValueWithNullableSumOnEmptySetReturnsNull()
		{
			using (var s = OpenSession())
			{
				var ageSum = s.Query<Person>()
					.Where(x => false) // make this an empty set
					.Select(x => x.Age)
					.ToFutureValue(x => x.Sum());

				Assert.IsNotNull(ageSum);
				Assert.IsNull(ageSum.Value);
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Person { Name = "Test1", Age = 20 });
				session.Save(new Person { Name = "Test2", Age = 30 });
				session.Save(new Person { Name = "Test3" });
				session.Save(new Person { Name = "Test4" });
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
