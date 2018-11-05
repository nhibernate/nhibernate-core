using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class MathGenerator : BaseHqlGeneratorForMethod
	{
		public MathGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => Math.Sin(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Cos(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Tan(default(double))),

				ReflectHelper.GetMethodDefinition(() => Math.Sinh(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Cosh(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Tanh(default(double))),

				ReflectHelper.GetMethodDefinition(() => Math.Asin(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Acos(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Atan(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Atan2(default(double), default(double))),

				ReflectHelper.GetMethodDefinition(() => Math.Sqrt(default(double))),

				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(float))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(long))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(int))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(short))),
				ReflectHelper.GetMethodDefinition(() => Math.Abs(default(sbyte))),

				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(double))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(float))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(long))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(int))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(short))),
				ReflectHelper.GetMethodDefinition(() => Math.Sign(default(sbyte))),

				ReflectHelper.GetMethodDefinition(() => Math.Floor(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => Math.Floor(default(double))),
				ReflectHelper.GetMethodDefinition(() => decimal.Floor(default(decimal))),

				ReflectHelper.GetMethodDefinition(() => Math.Ceiling(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => Math.Ceiling(default(double))),
				ReflectHelper.GetMethodDefinition(() => decimal.Ceiling(default(decimal))),

				ReflectHelper.GetMethodDefinition(() => Math.Pow(default(double), default(double))),

#if NETCOREAPP2_0
				ReflectHelper.GetMethodDefinition(() => MathF.Sin(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Cos(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Tan(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Sinh(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Cosh(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Tanh(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Asin(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Acos(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Atan(default(float))),
				ReflectHelper.GetMethodDefinition(() => MathF.Atan2(default(float), default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Sqrt(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Abs(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Sign(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Floor(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Ceiling(default(float))),

				ReflectHelper.GetMethodDefinition(() => MathF.Pow(default(float), default(float))),
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression expression, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string function = method.Name.ToLowerInvariant();

			if (function == "pow")
				function = "power";

			HqlExpression firstArgument = visitor.Visit(arguments[0]).AsExpression();

			if (arguments.Count == 2)
			{
				return treeBuilder.MethodCall(function, firstArgument, visitor.Visit(arguments[1]).AsExpression());
			}

			return treeBuilder.MethodCall(function, firstArgument);
		}
	}
}
