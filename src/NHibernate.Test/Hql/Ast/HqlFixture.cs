using System;
using System.Linq;
using System.Collections;
using NHibernate.Criterion;
using NHibernate.Engine.Query;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class HqlFixture : BaseFixture
	{
		protected HQLQueryPlan CreateQueryPlan(string hql, bool scalar)
		{
			return new QueryExpressionPlan(new StringQueryExpression(hql), scalar, new CollectionHelper.EmptyMapClass<string, IFilter>(), Sfi);
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
		public void OrderByPropertiesImplicitlySpecifiedInTheSelect()
		{
			// NH-2035 
			using (ISession s = OpenSession())
			{
				s.CreateQuery("select distinct z from Animal a join a.zoo as z order by z.name").List();
			}
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

		[Test]
		public void MultipleParametersInCaseStatement()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Animal { BodyWeight = 12, Description = "Polliwog" });
				s.Transaction.Commit();
			}

			try
			{
				using (ISession s = OpenSession())
				{
					var result = s.CreateQuery("select case when 'b' = ? then 2 when 'b' = ? then 1 else 0 end from Animal a")
						.SetParameter(0, "a")
						.SetParameter(1, "b")
						.SetMaxResults(1)
						.UniqueResult();
					Assert.AreEqual(1, result);
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (s.BeginTransaction())
				{
					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.Transaction.Commit();
				}
			}
		}

		[Test]
		public void ParameterInCaseThenClause()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Animal { BodyWeight = 12, Description = "Polliwog" });
				s.Transaction.Commit();
			}

			try
			{
				using (ISession s = OpenSession())
				{
					var result = s.CreateQuery("select case when 2=2 then ? else 0 end from Animal a")
						.SetParameter(0, 1)
						.UniqueResult();
					Assert.AreEqual(1, result);
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (s.BeginTransaction())
				{
					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.Transaction.Commit();
				}
			}
		}

		[Test]
		public void ParameterInCaseThenAndElseClausesWithCast()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Animal { BodyWeight = 12, Description = "Polliwog" });
				s.Transaction.Commit();
			}

			try
			{
				using (ISession s = OpenSession())
				{
					var result = s.CreateQuery("select case when 2=2 then cast(? as integer) else ? end from Animal a")
						.SetParameter(0, 1)
						.SetParameter(1, 0)
						.UniqueResult();
					Assert.AreEqual(1, result);
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (s.BeginTransaction())
				{
					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.Transaction.Commit();
				}
			}
		}

		[Test]
		public void SubselectAddition()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				s.Save(new Animal { BodyWeight = 12, Description = "Polliwog" });
				s.Transaction.Commit();
			}

			try
			{
				using (ISession s = OpenSession())
				{
					var result = s.CreateQuery("select count(a) from Animal a where (select count(a2) from Animal a2) + 1 > 1")
						.UniqueResult();
					Assert.AreEqual(1, result);
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (s.BeginTransaction())
				{
					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.Transaction.Commit();
				}
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

		[Test]
		public void InsertIntoFromSelect_WithSelectClauseParameters()
		{
			using (ISession s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					// arrange
					s.Save(new Animal() {Description = "cat1", BodyWeight = 2.1f});
					s.Save(new Animal() {Description = "cat2", BodyWeight = 2.5f});
					s.Save(new Animal() {Description = "cat3", BodyWeight = 2.7f});

					// act
					s.CreateQuery("insert into Animal (description, bodyWeight) select a.description, :weight from Animal a where a.bodyWeight < :weight")
						.SetParameter<float>("weight", 5.7f).ExecuteUpdate();

					// assert
					Assert.AreEqual(3, s.CreateCriteria<Animal>().SetProjection(Projections.RowCount())
					                    .Add(Restrictions.Gt("bodyWeight", 5.5f)).UniqueResult<int>());

					s.CreateQuery("delete from Animal").ExecuteUpdate();
					s.Transaction.Commit();
				}
			}
		}


		[Test]
		public void UnaryMinusBeforeParenthesesHandledCorrectly()
		{
			using (ISession s = OpenSession())
			using (ITransaction txn = s.BeginTransaction())
			{
				s.Save(new Animal {Description = "cat1", BodyWeight = 1});

				// NH-2290: Unary minus before parentheses wasn't handled correctly (this query returned 0).
				int actual = s.CreateQuery("select -(1+1) from Animal as h")
					.List<int>().Single();
				Assert.That(actual, Is.EqualTo(-2));

				// This was the workaround, which of course should still work.
				int actualWorkaround = s.CreateQuery("select -1*(1+1) from Animal as h")
					.List<int>().Single();
				Assert.That(actualWorkaround, Is.EqualTo(-2));
			}
		}
	}
}