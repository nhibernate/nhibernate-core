using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CriteriaQueryOnComponentCollection
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (s.BeginTransaction())
			{
				var parent = new Employee
				{
					Id = 2,
				};
				var emp = new Employee
				{
					Id = 1,
					Amounts = new HashSet<Money>
					{
						new Money {Amount = 9, Currency = "USD"},
						new Money {Amount = 3, Currency = "EUR"},
					},
					ManagedEmployees = new HashSet<ManagedEmployee>
					{
						new ManagedEmployee
						{
							Position = "parent",
							Employee = parent
						}
					}
				};
				s.Save(parent);
				s.Save(emp);

				s.Transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			using (s.BeginTransaction())
			{
				s.Delete("from System.Object");

				s.Transaction.Commit();
			}
		}

		[Test]
		public void CanQueryByCriteriaOnSetOfCompositeElement()
		{
			using (var s = Sfi.OpenSession())
			{
				var list = s.CreateCriteria<Employee>()
				            .CreateCriteria("ManagedEmployees")
				            .Add(Restrictions.Eq("Position", "parent"))
				            .SetResultTransformer(new RootEntityResultTransformer())
				            .List();
				Assert.That(list, Has.Count.EqualTo(1));
				Assert.That(list[0], Is.Not.Null);
				Assert.That(list[0], Is.TypeOf<Employee>());
				Assert.That(((Employee) list[0]).Id, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanQueryByCriteriaOnSetOfElement()
		{
			using (var s = Sfi.OpenSession())
			{
				var list = s.CreateCriteria<Employee>()
				            .CreateCriteria("Amounts")
				            .Add(Restrictions.Gt("Amount", 5m))
				            .SetResultTransformer(new RootEntityResultTransformer())
				            .List();
				Assert.That(list, Has.Count.EqualTo(1));
				Assert.That(list[0], Is.Not.Null);
				Assert.That(list[0], Is.TypeOf<Employee>());
				Assert.That(((Employee) list[0]).Id, Is.EqualTo(1));
			}
		}


		[TestCase(JoinType.LeftOuterJoin)]
		[TestCase(JoinType.InnerJoin)]
		public void CanQueryByCriteriaOnSetOfElementByCreateAlias(JoinType joinType)
		{
			using (var s = Sfi.OpenSession())
			{
				var list = s.CreateCriteria<Employee>("x")
				            .CreateAlias("x.Amounts", "amount", joinType)
				            .Add(Restrictions.Gt("amount.Amount", 5m))
				            .SetResultTransformer(new RootEntityResultTransformer())
				            .List();
				Assert.That(list, Has.Count.EqualTo(1));
				Assert.That(list[0], Is.Not.Null);
				Assert.That(list[0], Is.TypeOf<Employee>());
				Assert.That(((Employee) list[0]).Id, Is.EqualTo(1));
			}
		}


		[Test]
		public void CanQueryByCriteriaOnSetOfCompositeElement_UsingDetachedCriteria()
		{
			using (var s = Sfi.OpenSession())
			{
				var list = s.CreateCriteria<Employee>()
				            .Add(Subqueries.PropertyIn("id",
				                                       DetachedCriteria.For<Employee>()
				                                                       .SetProjection(Projections.Id())
				                                                       .CreateCriteria("Amounts")
				                                                       .Add(Restrictions.Gt("Amount", 5m))))
				            .List();
				Assert.That(list, Has.Count.EqualTo(1));
				Assert.That(list[0], Is.Not.Null);
				Assert.That(list[0], Is.TypeOf<Employee>());
				Assert.That(((Employee) list[0]).Id, Is.EqualTo(1));
			}
		}


		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.CriteriaQueryOnComponentCollection.Mappings.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

	}
}
