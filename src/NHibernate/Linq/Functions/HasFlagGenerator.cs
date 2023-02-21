using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	internal class HasFlagGenerator : BaseHqlGeneratorForMethod, IRuntimeMethodHqlGenerator
	{
		private const string _bitAndFunctionName = "band";

		public bool SupportsMethod(MethodInfo method)
		{
			return method.Name == nameof(Enum.HasFlag) && method.DeclaringType == typeof(Enum);
		}

		public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
		{
			return this;
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
