using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2658
{
	public static class ObjectExtensions
	{
		public static T GetProperty<T>(this object o, string propertyName)
		{
			// no implementation needed for this test
			throw new NotImplementedException();
		}
	}

	[TestFixture]
	public class Fixture : TestCase
	{
		public class DynamicPropertyGenerator : BaseHqlGeneratorForMethod
		{
			public DynamicPropertyGenerator()
			{
				// just registering for string here, but in a real implementation we'd be doing a runtime generator
				SupportedMethods = new[]
				{
					ReflectHelper.GetMethodDefinition(() => ObjectExtensions.GetProperty<string>(null, null))
				};
			}

			public override HqlTreeNode BuildHql(
				MethodInfo method,
				Expression targetObject,
				ReadOnlyCollection<Expression> arguments,
				HqlTreeBuilder treeBuilder,
				IHqlExpressionVisitor visitor)
			{
				var propertyName = (string) ((ConstantExpression) arguments[1]).Value;

				return treeBuilder.Dot(
					visitor.Visit(arguments[0]).AsExpression(),
					treeBuilder.Ident(propertyName)).AsExpression();
			}
		}

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] { "NHSpecificTest.NH2658.Mappings.hbm.xml" };

		protected override DebugSessionFactory BuildSessionFactory()
		{
			var sfi = base.BuildSessionFactory();

			// add our linq extension
			((ISessionFactoryImplementor)sfi).Settings.LinqToHqlGeneratorsRegistry.Merge(new DynamicPropertyGenerator());
			return sfi;
		}

		[Test]
		public void Does_Not_Cache_NonParameters()
		{
			using (var session = OpenSession())
			{
				// Passes
				using (var spy = new SqlLogSpy())
				{
					// Query by name
					(from p in session.Query<Product>() where p.GetProperty<string>("Name") == "Value" select p).ToList();

					var paramName = ((ISqlParameterFormatter) Sfi.ConnectionProvider.Driver).GetParameterName(0);
					Assert.That(spy.GetWholeLog(), Does.Contain("Name=" + paramName));
				}

				// Was failing.
				// Because this query was considered the same as the top query the hql will be reused from the top statement
				// even though GetProperty has a parameter that never get passed to sql or hql
				using (var spy = new SqlLogSpy())
				{
					// Query by description
					(from p in session.Query<Product>() where p.GetProperty<string>("Description") == "Value" select p).ToList();

					var paramName = ((ISqlParameterFormatter) Sfi.ConnectionProvider.Driver).GetParameterName(0);
					Assert.That(spy.GetWholeLog(), Does.Contain("Description=" + paramName));
				}
			}
		}
	}
}
