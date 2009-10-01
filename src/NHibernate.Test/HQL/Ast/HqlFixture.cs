using System;
using System.Collections;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.HQL.Ast
{
	[TestFixture]
	public class HqlFixture : BaseFixture
	{
		protected HQLQueryPlan CreateQueryPlan(string hql, bool scalar)
		{
			return new HQLQueryPlan(hql, scalar, new CollectionHelper.EmptyMapClass<string, IFilter>(), sessions);
		}

		protected HQLQueryPlan CreateQueryPlan(string hql)
		{
			return CreateQueryPlan(hql, false);
		}

		private static void Check(ReturnMetadata returnMetadata, bool expectingEmptyTypes, bool expectingEmptyAliases)
		{
			Assert.IsNotNull(returnMetadata, "null return metadata");
			Assert.IsNotNull(returnMetadata, "null return metadata - types");
			Assert.AreEqual(1, returnMetadata.ReturnTypes.Length, "unexpected return size");

			if (expectingEmptyTypes)
			{
				Assert.IsNull(returnMetadata.ReturnTypes[0], "non-empty types");
			}
			else
			{
				Assert.IsNotNull(returnMetadata.ReturnTypes[0], "empty types");
			}

			if (expectingEmptyAliases)
			{
				Assert.IsNull(returnMetadata.ReturnAliases, "non-empty aliases");
			}
			else
			{
				Assert.IsNotNull(returnMetadata.ReturnAliases, "empty aliases");
				Assert.IsNotNull(returnMetadata.ReturnAliases[0], "empty aliases");
			}
		}

		[Test]
		public void ReturnMetadata()
		{
			HQLQueryPlan plan;
			plan = CreateQueryPlan("from Animal a");
			Check(plan.ReturnMetadata, false, true);

			plan = CreateQueryPlan("select a as animal from Animal a");
			Check(plan.ReturnMetadata, false, false);

			plan = CreateQueryPlan("from System.Object");
			Check(plan.ReturnMetadata, true, true);

			plan = CreateQueryPlan("select o as entity from System.Object o");
			Check(plan.ReturnMetadata, true, false);
		}

		[Test]
		public void CaseClauseInSelect()
		{
			// NH-322
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Animal {BodyWeight = 12, Description = "Polliwog"});
				s.Transaction.Commit();
			}
		
			using (ISession s = OpenSession())
			{
				var l = s.CreateQuery("select a.id, case when a.description = 'Polliwog' then 2 else 0 end from Animal a").List();
				var element = (IList)l[0];
				Assert.That(element[1], Is.EqualTo(2));

				// work with alias
				l = s.CreateQuery("select a.id, case when a.description = 'Polliwog' then 2 else 0 end as value from Animal a").List();
				element = (IList)l[0];
				Assert.That(element[1], Is.EqualTo(2));
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.Transaction.Commit();
			}
		}

		[Test, Ignore("Not fixed yet.")]
		public void SumShouldReturnDouble()
		{
			// NH-1734
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Human{ IntValue = 11, BodyWeight = 12.5f, Description = "Polliwog" });
				s.Transaction.Commit();
			}

			using (ISession s = OpenSession())
			{
				var l = s.CreateQuery("select sum(a.intValue * a.bodyWeight) from Animal a group by a.id").List();
				Assert.That(l[0], Is.InstanceOf<Double>());
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.CreateQuery("delete from Animal").ExecuteUpdate();
				s.Transaction.Commit();
			}
		}

		[Test]
		public void CanParseMaxLong()
		{
			// NH-1833
			using (ISession s = OpenSession())
			{
				s.CreateQuery(string.Format("from SimpleClass sc where sc.LongValue = {0}", long.MaxValue)).List();
				s.CreateQuery(string.Format("from SimpleClass sc where sc.LongValue = {0}L", long.MaxValue)).List();
				s.CreateQuery(string.Format("from SimpleClass sc where sc.LongValue = 123L")).List();
				s.CreateQuery(string.Format("from SimpleClass sc where sc.LongValue = 123")).List();
				s.CreateQuery(string.Format("from SimpleClass sc where sc.LongValue = {0}", int.MaxValue + 1L)).List();
			}
		}

		[Test]
		public void InvalidJoinOnProperty()
		{
			// NH-1915
			using (ISession s = OpenSession())
			{
				Assert.Throws<InvalidPathException>(
					() =>
					{
						s.CreateQuery("from Zoo z inner join fetch z.classification").List();
					},
					"Incorrect path not caught during parsing");
			}
		}
	}
}