using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	internal class HasFlagGenerator : BaseHqlGeneratorForMethod
	{
		private const string _bitAndFunctionName = "band";

		public HasFlagGenerator()
		{
			SupportedMethods = new[] { ReflectHelper.GetMethodDefinition<Enum>(x => x.HasFlag(default)) };
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Equality(
						treeBuilder.MethodCall(
							_bitAndFunctionName,
							 visitor.Visit(targetObject).AsExpression(),
							 visitor.Visit(arguments[0]).AsExpression()),
						visitor.Visit(arguments[0]).AsExpression());
		}
	}
}
