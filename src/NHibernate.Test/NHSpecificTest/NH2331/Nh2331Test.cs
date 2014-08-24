using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2331
{
	public class Bar
	{
		public string Name { get; set; }
		public double Sum { get; set; }
	}

	[TestFixture]
	public class Nh2331Test : BugTestCase
	{
		private Guid person0Id;
		private Guid person1Id;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			var person0 = new Person
			              	{
			              		Name = "Schorsch",
			              	};

			var person1 = new Person
			              	{
			              		Name = "Sepp",
			              	};

			var person2 = new Person
			              	{
			              		Name = "Detlef",
			              	};

			var forum0 = new Forum
			             	{
			             		Name = "Oof",
			             		Dollars = 1887.00,
			             	};

			var forum1 = new Forum
			             	{
			             		Name = "Rab",
			             		Dollars = 33.00,
			             	};

			var forum2 = new Forum
			             	{
			             		Name = "Main",
			             		Dollars = 42.42,
			             	};

			var group0 = new MemberGroup
			             	{
			             		Name = "Gruppe Bla",
			             		Members = new List<Person>(),
			             		Forums = new List<Forum>(),
			             	};
			group0.Members.Add(person0);
			group0.Forums.Add(forum0);
			group0.Forums.Add(forum1);

			var group1 = new MemberGroup
			             	{
			             		Name = "Gruppe Blub",
			             		Members = new List<Person>(),
			             		Forums = new List<Forum>(),
			             	};
			group1.Members.Add(person1);
			group1.Members.Add(person2);
			group1.Forums.Add(forum2);

			using (ISession session = OpenSession())
			{
				person0Id = (Guid) session.Save(person0);
				person1Id = (Guid) session.Save(person1);

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return (true);
		}

		[Test]
		public void DetachedCriteriaCorrelatedQueryExplodes()
		{
			using (ISession session = OpenSession())
			{
			    DetachedCriteria memberGroupCriteria
			        = DetachedCriteria
			            .For<MemberGroup>()
			            .CreateAlias("Members", "m")
			            .CreateAlias("Forums", "f")
			            .Add(Restrictions.EqProperty("m.Id", "p.Id"))
			            .SetProjection(Projections.Property("f.Id"))
			        ;

			    var ids = new List<Guid>();
			    ids.Add(person0Id);
			    ids.Add(person1Id);

			    DetachedCriteria forumCriteria
			        = DetachedCriteria
			            .For<Forum>("fff")
			            .Add(Restrictions.NotEqProperty("Id", "p.Id"))
			            .Add(Subqueries.PropertyIn("Id", memberGroupCriteria))
			            .SetProjection
			            (
			                Projections.Sum("Dollars")
			            )
			        ;

			    DetachedCriteria personCriteria
			        = DetachedCriteria
			            .For<Person>("p")
			            .Add(Restrictions.InG("Id", ids))
			            .SetProjection
			            (
			                Projections
			                    .ProjectionList()
			                    .Add(Projections.Property("Name"), "Name")
			                    .Add(Projections.SubQuery(forumCriteria), "Sum")
			            )
			            .SetResultTransformer(Transformers.AliasToBean(typeof (Bar)))
			        ;

			    ICriteria criteria = personCriteria.GetExecutableCriteria(session);
                criteria.Executing(c => c.List()).NotThrows();
			}
		}
	}
}