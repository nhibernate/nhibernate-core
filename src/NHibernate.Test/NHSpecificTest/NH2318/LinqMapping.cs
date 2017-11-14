using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.NH2318
{
	public static class TrimExtensions
	{
		public static string TrimLeading(this string source, string trim)
		{
			// Bogus implementation so we know for sure it's the database doing the work.
			throw new NotImplementedException();
		}

		public static string TrimTrailing(this string source, string trim)
		{
			// Bogus implementation so we know for sure it's the database doing the work.
			throw new NotImplementedException();
		}
	}
	public class TrimGenerator : BaseHqlGeneratorForMethod
	{
		public TrimGenerator()
		{
			SupportedMethods = new[] {
				ReflectHelper.GetMethodDefinition(() => TrimExtensions.TrimLeading(null, null)),
				ReflectHelper.GetMethodDefinition(() => TrimExtensions.TrimTrailing(null, null)),
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string leadingOrTrailing = "TRAILING";
			if (method.Name == "TrimLeading")
				leadingOrTrailing = "LEADING";

			return treeBuilder.MethodCall("Trim",
										  treeBuilder.Ident(leadingOrTrailing),
			                              visitor.Visit(arguments[1]).AsExpression(),
										  treeBuilder.Ident("FROM"),
			                              visitor.Visit(arguments[0]).AsExpression());
		}
	}

	public class ExtendedLinqtoHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public ExtendedLinqtoHqlGeneratorsRegistry()
		{
			this.Merge(new TrimGenerator());
		}
	}
}
