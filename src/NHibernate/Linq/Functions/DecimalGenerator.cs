using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class DecimalAddGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalAddGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Add(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Add(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalDivideGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalDivideGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Divide(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.Cast(treeBuilder.Divide(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalMultiplyGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalMultiplyGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Multiply(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Multiply(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalSubtractGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalSubtractGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Subtract(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Subtract(visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalRemainderGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalRemainderGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Remainder(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.MethodCall("mod", visitor.Visit(arguments[0]).AsExpression(), visitor.Visit(arguments[1]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalNegateGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalNegateGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Negate(default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.TransparentCast(treeBuilder.Negate(visitor.Visit(arguments[0]).AsExpression()), typeof(decimal));
		}
	}

	public class DecimalFloorGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalFloorGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Floor(default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("floor", visitor.Visit(arguments[0]).AsExpression());
		}
	}

	public class DecimalCeilingGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalCeilingGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Ceiling(default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("ceiling", visitor.Visit(arguments[0]).AsExpression());
		}
	}

	public class DecimalRoundGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalRoundGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal), default(int))),
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			HqlExpression numberOfDecimals = (arguments.Count == 2) ? visitor.Visit(arguments[1]).AsExpression() : treeBuilder.Constant(0);
			return treeBuilder.TransparentCast(treeBuilder.MethodCall("round", visitor.Visit(arguments[0]).AsExpression(), numberOfDecimals), typeof(decimal));
		}
	}
}
