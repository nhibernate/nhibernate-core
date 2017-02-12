using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public static class BooleanLinqExtensions
	{
		public static bool FreeText(this string source, string pattern)
		{
			throw new NotImplementedException();
		}
	}

	public class FreetextGenerator : BaseHqlGeneratorForMethod
	{
		public FreetextGenerator()
		{
			SupportedMethods = new[] {ReflectionHelper.GetMethodDefinition(() => BooleanLinqExtensions.FreeText(null, null))};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
											 ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder,
											 IHqlExpressionVisitor visitor)
		{
			IEnumerable<HqlExpression> args = arguments.Select(a => visitor.Visit(a))
				.Cast<HqlExpression>();

			return treeBuilder.BooleanMethodCall("FREETEXT", args);
		}
	}

	[TestFixture]
	public class BooleanMethodExtensionExample : LinqTestCase
	{
		public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
		{
			public MyLinqToHqlGeneratorsRegistry()
			{
				RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => BooleanLinqExtensions.FreeText(null, null)),
								  new FreetextGenerator());
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();
		}

		[Test, Ignore("It work only with full-text indexes enabled.")]
		public void CanUseMyCustomExtension()
		{
			List<Customer> contacts = (from c in db.Customers where c.ContactName.FreeText("Thomas") select c).ToList();
			Assert.That(contacts.Count, Is.GreaterThan(0));
			Assert.That(contacts.Select(c => c.ContactName).All(c => c.Contains("Thomas")), Is.True);
		}
	}
}