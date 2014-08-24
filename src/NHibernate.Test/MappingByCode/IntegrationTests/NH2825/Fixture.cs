using System.Collections;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2825
{
	/// <summary>
	/// Demonstrates a bi-directional many-to-one mapping using property-ref.
	/// </summary>
	/// <remarks>
	/// See <see cref="FixtureByCode"/> for the by-code mapped version. This class
	/// demonstrates the equivalent XML mapping.
	/// </remarks>
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "MappingByCode.IntegrationTests.NH2825.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var child1 = new Child { Name = "Child 1" };
				var child2 = new Child { Name = "Child 2" };
				var child3 = new Child { Name = "Child 3" };

				var parent1 = new Parent { Name = "Parent 1", ParentCode = 10 };
				var parent2 = new Parent { Name = "Parent 2", ParentCode = 20 };

				session.Save(parent1);
				session.Save(parent2);

				parent1.AddChild(child1);
				parent1.AddChild(child2);
				parent2.AddChild(child3);

				session.Save(child1);
				session.Save(child2);
				session.Save(child3);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from Child");
				session.Delete("from Parent");
				transaction.Commit();
			}	
		}

		[Test]
		public void VerifyOneEndOfManyToOneMappingUsingPropertyRef()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Child>().ToList();
				Assert.That(result.Count, Is.EqualTo(3));
				Assert.That(result.Where(c => c.Name == "Child 1").First().Parent.ParentCode, Is.EqualTo(10));
			}
		}

		[Test]
		public void VerifyManyEndOfManyToOneMappingUsingPropertyRef()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Parent>().ToList();
				Assert.That(result.Count(), Is.EqualTo(2));
				Assert.That(result.First(p => p.ParentCode == 10).Children.Count(), Is.EqualTo(2));
			}
		}
	}
}