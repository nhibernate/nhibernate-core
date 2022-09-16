using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
    internal class TruncateGenerator : BaseHqlGeneratorForMethod
	{
		public TruncateGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(Math.Truncate, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Truncate, default(double)),
				ReflectHelper.FastGetMethod(decimal.Truncate, default(decimal)),

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				ReflectHelper.FastGetMethod(MathF.Truncate, default(float)),
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression expression, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("truncate", visitor.Visit(arguments[0]).AsExpression(), treeBuilder.Constant(0));
		}
	}
}
