using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public class CoalesceChildThenAccessMethod : GH1879BaseFixture<Issue>
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var clientA = new Client { Name = "Albert" };
				var clientB = new Client { Name = "Bob" };
				var corpA = new CorporateClient { Name = "Alpha", CorporateId = "1234" };
				var corpB = new CorporateClient { Name = "Beta", CorporateId = "5647" };
				var clientZ = new Client { Name = null }; // A null value should propagate if the entity is non-null
				session.Save(clientA);
				session.Save(clientB);
				session.Save(corpA);
				session.Save(corpB);
				session.Save(clientZ);
				
				var projectA = new Project { Name = "A", BillingClient = null,  Client = clientA };
				var projectB = new Project { Name = "B", BillingClient = corpB,  Client = clientA };
				var projectC = new Project { Name = "C", BillingClient = null,  Client = clientB };
				var projectD = new Project { Name = "D", BillingClient = corpA,  Client = clientB };
				var projectE = new Project { Name = "E", BillingClient = clientZ, Client = clientA };
				var projectZ = new Project { Name = "Z", BillingClient = null,  Client = null };
				session.Save(projectA);
				session.Save(projectB);
				session.Save(projectC);
				session.Save(projectD);
				session.Save(projectE);
				session.Save(projectZ);
 
				session.Save(new Issue { Name = "01", Project = null, Client = null });
				session.Save(new Issue { Name = "02", Project = null, Client = clientA });
				session.Save(new Issue { Name = "03", Project = null, Client = clientB });
				session.Save(new Issue { Name = "04", Project = projectC, Client = clientA });
				session.Save(new Issue { Name = "05", Project = projectA, Client = clientB });
				session.Save(new Issue { Name = "06", Project = projectB, Client = clientA });
				session.Save(new Issue { Name = "07", Project = projectD, Client = clientB });
				session.Save(new Issue { Name = "08", Project = projectZ, Client = corpA });
				session.Save(new Issue { Name = "09", Project = projectZ, Client = corpB });
				session.Save(new Issue { Name = "10", Project = projectE, Client = clientA });

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.ByCode().LinqToHqlGeneratorsRegistry<TestLinqToHqlGeneratorsRegistry>();
		}

		private class TestLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
		{
			public TestLinqToHqlGeneratorsRegistry()
			{
				this.Merge(new TestHqlGeneratorForMethod());
			}
		}

		private class TestHqlGeneratorForMethod : IHqlGeneratorForMethod
		{
			/// <inheritdoc />
			public IEnumerable<MethodInfo> SupportedMethods => new []
			{
				ReflectHelper.GetMethodDefinition<Client>(x => x.NameByMethod()),
			};

			/// <inheritdoc />
			public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.Dot(visitor.Visit(targetObject).AsExpression(), treeBuilder.Ident("Name").AsExpression());
			}
		}

		[Test]
		public void WhereClause()
		{
			AreEqual(
				// Actual
				q => q.Where(i => (i.Project.BillingClient ?? i.Project.Client ?? i.Client).NameByMethod().StartsWith("A")),
				// Expected
				q => q.Where(i => (i.Project.BillingClient != null ? i.Project.BillingClient.NameByMethod() : i.Project.Client != null ? i.Project.Client.NameByMethod() : i.Client.NameByMethod()).StartsWith("A"))
			);
		}

		[Test]
		public void SelectClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(i =>i.Name)
				      .Select(i => (i.Project.BillingClient ?? i.Project.Client ?? i.Client).NameByMethod()),
				// Expected
				q => q.OrderBy(i =>i.Name)
				      .Select(i => i.Project.BillingClient != null ? i.Project.BillingClient.NameByMethod() : i.Project.Client != null ? i.Project.Client.NameByMethod() : i.Client.NameByMethod())
			);
		}
		
		[Test]
		public void SelectClauseToAnon()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(i =>i.Name)
				      .Select(i => new { Key =i.Name, Client = (i.Project.BillingClient ?? i.Project.Client ?? i.Client).NameByMethod() }),
				// Expected
				q => q.OrderBy(i =>i.Name)
				      .Select(i => new { Key =i.Name, Client = i.Project.BillingClient != null ? i.Project.BillingClient.NameByMethod() : i.Project.Client != null ? i.Project.Client.NameByMethod() : i.Client.NameByMethod() })
			);
		}

		[Test]
		public void OrderByClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(i => (i.Project.BillingClient ?? i.Project.Client ?? i.Client).NameByMethod() ?? "ZZZ")
				      .ThenBy(i =>i.Name)
				      .Select(i =>i.Name),
				// Expected
				q => q.OrderBy(i => (i.Project.BillingClient != null ? i.Project.BillingClient.NameByMethod() : i.Project.Client != null ? i.Project.Client.NameByMethod() : i.Client.NameByMethod()) ?? "ZZZ")
				      .ThenBy(i =>i.Name)
				      .Select(i =>i.Name)
			);
		}

		[Test]
		public void GroupByClause()
		{
			AreEqual(
				// Actual
				q => q.GroupBy(i => (i.Project.BillingClient ?? i.Project.Client ?? i.Client).NameByMethod())
				      .OrderBy(x => x.Key ?? "ZZZ")
				      .Select(grp => new  { grp.Key, Count = grp.Count() }),
				// Expected
				q => q.GroupBy(i => i.Project.BillingClient != null ? i.Project.BillingClient.NameByMethod() : i.Project.Client != null ? i.Project.Client.NameByMethod() : i.Client.NameByMethod())
				      .OrderBy(x => x.Key ?? "ZZZ")
				      .Select(grp => new  { grp.Key, Count = grp.Count() })
			);
		}
	}
}
