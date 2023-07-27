using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	internal class RoundGenerator : BaseHqlGeneratorForMethod
	{
		public RoundGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(Math.Round, default(double)),
				ReflectHelper.FastGetMethod(Math.Round, default(double), default(int)),
				ReflectHelper.FastGetMethod(Math.Round, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Round, default(decimal), default(int)),
				ReflectHelper.FastGetMethod(decimal.Round, default(decimal)),
				ReflectHelper.FastGetMethod(decimal.Round, default(decimal), default(int)),

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				ReflectHelper.FastGetMethod(MathF.Round, default(float)),
				ReflectHelper.FastGetMethod(MathF.Round, default(float), default(int)),
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var numberOfDecimals = arguments.Count == 2
				? visitor.Visit(arguments[1]).AsExpression()
				: treeBuilder.Constant(0);
			return treeBuilder.TransparentCast(
				treeBuilder.MethodCall("round", visitor.Visit(arguments[0]).AsExpression(), numberOfDecimals),
				method.ReturnType);
		}
	}
}
