using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1766
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var parent = new A
				{
					Items = new List<B>
					{
						new B(),
						new B(),
					},
				};

				foreach (var child in parent.Items)
				{
					child.A = parent;
				}

				session.Persist(parent);

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

		[Test]
		public void ShouldSetListIndexForInverseList()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<A>().ToList();
				Assert.That(result, Has.Count.EqualTo(1));

				var root = result.Single();
				Assert.That(root.Items, Is.Not.Null);
				Assert.That(root.Items, Has.Count.EqualTo(2));

				var expectedIndex = 0;
				foreach (var child in root.Items)
				{
					Assert.That(child.ListIndex, Is.EqualTo(expectedIndex));
					++expectedIndex;
				}
			}
		}
	}
}
