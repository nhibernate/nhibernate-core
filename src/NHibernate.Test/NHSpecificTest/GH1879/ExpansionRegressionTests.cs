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
	public class ExpansionRegressionTests : GH1879BaseFixture<Invoice>
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Invoice { InvoiceNumber = 1, Amount = 10, SpecialAmount = 100, Paid = false });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 100, Paid = true });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 110, Paid = false });
				session.Save(new Invoice { InvoiceNumber = 2, Amount = 10, SpecialAmount = 110, Paid = true });

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
				this.Merge(new ObjectEquality());
			}
		}

		private class ObjectEquality : IHqlGeneratorForMethod
		{
			public HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
			{
				return treeBuilder.Equality(visitor.Visit(targetObject).AsExpression(), visitor.Visit(arguments[0]).AsExpression());
			}

			public IEnumerable<MethodInfo> SupportedMethods
			{
				get
				{
					yield return ReflectHelper.GetMethodDefinition<object>(x => x.Equals(x));
				}
			}
		}

		[Test]
		public void MethodShouldNotExpandForNonConditionalOrCoalesce()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.Amount + e.SpecialAmount)).Equals(110)), Is.EqualTo(2));
			}
		}

		[Test]
		public void MethodShouldNotExpandForConditionalWithPropertyAccessor()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.Paid ? e.Amount : e.SpecialAmount)).Equals(10)), Is.EqualTo(2));
			}
		}

		[Test]
		public void MethodShouldNotExpandForCoalesceWithPropertyAccessor()
		{
			using (var session = OpenSession())
			{
				Assert.That(session.Query<Invoice>().Count(e => ((object)(e.SpecialAmount ?? e.Amount)).Equals(100)), Is.EqualTo(2));
			}
		}
	}
}
