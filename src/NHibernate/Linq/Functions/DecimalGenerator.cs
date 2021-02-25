using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	internal class DecimalAddGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalAddGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Add, default(decimal), default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Add(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	internal class DecimalDivideGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalDivideGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Divide, default(decimal), default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Cast(treeBuilder.Divide(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	internal class DecimalMultiplyGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalMultiplyGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Multiply, default(decimal), default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Multiply(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	internal class DecimalSubtractGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalSubtractGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Subtract, default(decimal), default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Subtract(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	internal class DecimalRemainderGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalRemainderGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Remainder, default(decimal), default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.MethodCall("mod", visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	internal class DecimalNegateGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalNegateGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.FastGetMethod(decimal.Negate, default(decimal))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Negate(visitor.Visit(arguments[0]).AsExpression()), typeof(decimal));
		}
	}
}
