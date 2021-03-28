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
				ReflectHelper.FastGetMethod(Math.Sin, default(double)),
				ReflectHelper.FastGetMethod(Math.Cos, default(double)),
				ReflectHelper.FastGetMethod(Math.Tan, default(double)),

				ReflectHelper.FastGetMethod(Math.Sinh, default(double)),
				ReflectHelper.FastGetMethod(Math.Cosh, default(double)),
				ReflectHelper.FastGetMethod(Math.Tanh, default(double)),

				ReflectHelper.FastGetMethod(Math.Asin, default(double)),
				ReflectHelper.FastGetMethod(Math.Acos, default(double)),
				ReflectHelper.FastGetMethod(Math.Atan, default(double)),
				ReflectHelper.FastGetMethod(Math.Atan2, default(double), default(double)),

				ReflectHelper.FastGetMethod(Math.Sqrt, default(double)),

				ReflectHelper.FastGetMethod(Math.Abs, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Abs, default(double)),
				ReflectHelper.FastGetMethod(Math.Abs, default(float)),
				ReflectHelper.FastGetMethod(Math.Abs, default(long)),
				ReflectHelper.FastGetMethod(Math.Abs, default(int)),
				ReflectHelper.FastGetMethod(Math.Abs, default(short)),
				ReflectHelper.FastGetMethod(Math.Abs, default(sbyte)),

				ReflectHelper.FastGetMethod(Math.Sign, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Sign, default(double)),
				ReflectHelper.FastGetMethod(Math.Sign, default(float)),
				ReflectHelper.FastGetMethod(Math.Sign, default(long)),
				ReflectHelper.FastGetMethod(Math.Sign, default(int)),
				ReflectHelper.FastGetMethod(Math.Sign, default(short)),
				ReflectHelper.FastGetMethod(Math.Sign, default(sbyte)),

				ReflectHelper.FastGetMethod(Math.Floor, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Floor, default(double)),
				ReflectHelper.FastGetMethod(decimal.Floor, default(decimal)),

				ReflectHelper.FastGetMethod(Math.Ceiling, default(decimal)),
				ReflectHelper.FastGetMethod(Math.Ceiling, default(double)),
				ReflectHelper.FastGetMethod(decimal.Ceiling, default(decimal)),

				ReflectHelper.FastGetMethod(Math.Pow, default(double), default(double)),

#if NETCOREAPP2_0
				ReflectHelper.FastGetMethod(MathF.Sin, default(float)),
				ReflectHelper.FastGetMethod(MathF.Cos, default(float)),
				ReflectHelper.FastGetMethod(MathF.Tan, default(float)),

				ReflectHelper.FastGetMethod(MathF.Sinh, default(float)),
				ReflectHelper.FastGetMethod(MathF.Cosh, default(float)),
				ReflectHelper.FastGetMethod(MathF.Tanh, default(float)),

				ReflectHelper.FastGetMethod(MathF.Asin, default(float)),
				ReflectHelper.FastGetMethod(MathF.Acos, default(float)),
				ReflectHelper.FastGetMethod(MathF.Atan, default(float)),
				ReflectHelper.FastGetMethod(MathF.Atan2, default(float), default(float)),

				ReflectHelper.FastGetMethod(MathF.Sqrt, default(float)),

				ReflectHelper.FastGetMethod(MathF.Abs, default(float)),

				ReflectHelper.FastGetMethod(MathF.Sign, default(float)),

				ReflectHelper.FastGetMethod(MathF.Floor, default(float)),

				ReflectHelper.FastGetMethod(MathF.Ceiling, default(float)),

				ReflectHelper.FastGetMethod(MathF.Pow, default(float), default(float)),
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression expression, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string function = method.Name.ToLowerInvariant();

			if (function == "pow")
				function = "power";

			var firstArgument = visitor.Visit(arguments[0]).AsExpression();

			if (arguments.Count == 2)
			{
				return treeBuilder.MethodCall(function, firstArgument, visitor.Visit(arguments[1]).AsExpression());
			}

			return treeBuilder.MethodCall(function, firstArgument);
		}
	}
}
