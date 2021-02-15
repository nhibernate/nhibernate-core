using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1040
{
	[TestFixture]
	public class SubclassPropertyRefFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var child = new Child();
				session.Save(child);

				var consumer = new Consumer();
				session.Save(consumer);
				child.UK = consumer.Id;
				consumer.Child = child;

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CanLazyLoad()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from e in session.Query<Consumer>()
							select e).First();

				Assert.That(result.Child, Is.Not.Null);
				Assert.DoesNotThrow(() => NHibernateUtil.Initialize(result.Child));
			}
		}

		[Test]
		public void CanLinqFetch()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from e in session.Query<Consumer>().Fetch(x => x.Child)
							select e).First();

				Assert.That(result.Child, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(result.Child), Is.True);
			}
		}

		[Test]
		public void CanQueryOverFetch()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = (from e in session.Query<Consumer>().Fetch(x => x.Child)
							select e).First();

				Assert.That(result.Child, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(result.Child), Is.True);
			}
		}
	}
}
