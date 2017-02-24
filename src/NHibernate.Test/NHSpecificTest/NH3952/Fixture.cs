using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3952
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", ParentId = e1.Id, Hobbies = new[] { "Inline skate", "Sailing" } };
				session.Save(e2);

				var e3 = new Entity { Name = "Max", ParentId = e1.Id };
				session.Save(e3);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void SimpleNestedSelect()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => new { e.Name, children = e.Children.Select(c => c.Name).ToArray() })
					.OrderBy(e => e.Name)
					.ToList();

				Assert.AreEqual(3, result.Count);
				Assert.AreEqual(2, result[0].children.Length);
				Assert.AreEqual("Bob", result[0].Name);
				Assert.Contains("Max", result[0].children);
				Assert.Contains("Sally", result[0].children);
				Assert.AreEqual(0, result[1].children.Length + result[2].children.Length);
			}
		}

		[Test]
		public void ArraySelect()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Select(e => new { e.Name, e.Hobbies })
					.OrderBy(e => e.Name)
					.ToList();

				Assert.AreEqual(3, result.Count);
				Assert.AreEqual(2, result[2].Hobbies.Length);
				Assert.AreEqual("Sally", result[2].Name);
				Assert.Contains("Inline skate", result[2].Hobbies);
				Assert.Contains("Sailing", result[2].Hobbies);
				Assert.AreEqual(0, result[0].Hobbies.Length + result[1].Hobbies.Length);
			}
		}
	}
}