using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.GH2529
{
	public class DateTimeAddYearsMethodHqlGenerator : BaseHqlGeneratorForMethod
	{
		public DateTimeAddYearsMethodHqlGenerator()
		{
			SupportedMethods = new[] {
				ReflectHelper.GetMethodDefinition((DateTime x) => x.AddYears(0))
			};
		}

		public override HqlTreeNode BuildHql(
			MethodInfo method,
			Expression targetObject,
			ReadOnlyCollection<Expression> arguments,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor
		)
		{
			return treeBuilder.MethodCall(
				nameof(DateTime.AddYears),
				visitor.Visit(targetObject)
					.AsExpression(),
				visitor.Visit(arguments[0])
					.AsExpression()
			);
		}
	}
}
