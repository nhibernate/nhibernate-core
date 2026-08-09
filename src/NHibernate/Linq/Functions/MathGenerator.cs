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

				ReflectHelper.FastGetMethod(Math.Min, default(decimal), default(decimal)),
				ReflectHelper.FastGetMethod(Math.Min, default(double), default(double)),
				ReflectHelper.FastGetMethod(Math.Min, default(float), default(float)),
				ReflectHelper.FastGetMethod(Math.Min, default(long), default(long)),
				ReflectHelper.FastGetMethod(Math.Min, default(ulong), default(ulong)),
				ReflectHelper.FastGetMethod(Math.Min, default(int), default(int)),
				ReflectHelper.FastGetMethod(Math.Min, default(uint), default(uint)),
				ReflectHelper.FastGetMethod(Math.Min, default(short), default(short)),
				ReflectHelper.FastGetMethod(Math.Min, default(ushort), default(ushort)),
				ReflectHelper.FastGetMethod(Math.Min, default(sbyte), default(sbyte)),
				ReflectHelper.FastGetMethod(Math.Min, default(byte), default(byte)),

				ReflectHelper.FastGetMethod(Math.Max, default(decimal), default(decimal)),
				ReflectHelper.FastGetMethod(Math.Max, default(double), default(double)),
				ReflectHelper.FastGetMethod(Math.Max, default(float), default(float)),
				ReflectHelper.FastGetMethod(Math.Max, default(long), default(long)),
				ReflectHelper.FastGetMethod(Math.Max, default(ulong), default(ulong)),
				ReflectHelper.FastGetMethod(Math.Max, default(int), default(int)),
				ReflectHelper.FastGetMethod(Math.Max, default(uint), default(uint)),
				ReflectHelper.FastGetMethod(Math.Max, default(short), default(short)),
				ReflectHelper.FastGetMethod(Math.Max, default(ushort), default(ushort)),
				ReflectHelper.FastGetMethod(Math.Max, default(sbyte), default(sbyte)),
				ReflectHelper.FastGetMethod(Math.Max, default(byte), default(byte)),

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
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

				ReflectHelper.FastGetMethod(MathF.Min, default(float), default(float)),
				ReflectHelper.FastGetMethod(MathF.Max, default(float), default(float)),
#endif
#if NET8_0_OR_GREATER
				ReflectHelper.FastGetMethod(float.Sin, default(float)),
				ReflectHelper.FastGetMethod(float.Cos, default(float)),
				ReflectHelper.FastGetMethod(float.Tan, default(float)),
				ReflectHelper.FastGetMethod(float.Sinh, default(float)),
				ReflectHelper.FastGetMethod(float.Cosh, default(float)),
				ReflectHelper.FastGetMethod(float.Tanh, default(float)),
				ReflectHelper.FastGetMethod(float.Asin, default(float)),
				ReflectHelper.FastGetMethod(float.Acos, default(float)),
				ReflectHelper.FastGetMethod(float.Atan, default(float)),
				ReflectHelper.FastGetMethod(float.Atan2, default(float), default(float)),
				ReflectHelper.FastGetMethod(float.Sqrt, default(float)),
				ReflectHelper.FastGetMethod(float.Abs, default(float)),
				ReflectHelper.FastGetMethod(float.Sign, default(float)),
				ReflectHelper.FastGetMethod(float.Floor, default(float)),
				ReflectHelper.FastGetMethod(float.Ceiling, default(float)),
				ReflectHelper.FastGetMethod(float.Pow, default(float), default(float)),
				ReflectHelper.FastGetMethod(float.Min, default(float), default(float)),
				ReflectHelper.FastGetMethod(float.Max, default(float), default(float)),

				ReflectHelper.FastGetMethod(double.Sin, default(double)),
				ReflectHelper.FastGetMethod(double.Cos, default(double)),
				ReflectHelper.FastGetMethod(double.Tan, default(double)),
				ReflectHelper.FastGetMethod(double.Sinh, default(double)),
				ReflectHelper.FastGetMethod(double.Cosh, default(double)),
				ReflectHelper.FastGetMethod(double.Tanh, default(double)),
				ReflectHelper.FastGetMethod(double.Asin, default(double)),
				ReflectHelper.FastGetMethod(double.Acos, default(double)),
				ReflectHelper.FastGetMethod(double.Atan, default(double)),
				ReflectHelper.FastGetMethod(double.Atan2, default(double), default(double)),
				ReflectHelper.FastGetMethod(double.Sqrt, default(double)),
				ReflectHelper.FastGetMethod(double.Abs, default(double)),
				ReflectHelper.FastGetMethod(double.Sign, default(double)),
				ReflectHelper.FastGetMethod(double.Floor, default(double)),
				ReflectHelper.FastGetMethod(double.Ceiling, default(double)),
				ReflectHelper.FastGetMethod(double.Pow, default(double), default(double)),
				ReflectHelper.FastGetMethod(double.Min, default(double), default(double)),
				ReflectHelper.FastGetMethod(double.Max, default(double), default(double)),
#endif
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression expression, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var function = method.Name.ToLowerInvariant() switch
			{
				"pow" => "power",
				// min and max are aggregates in HQL, their scalar counterparts are named greatest and least.
				"min" => "least",
				"max" => "greatest",
				var f => f,
			};

			var firstArgument = visitor.Visit(arguments[0]).AsExpression();

			if (arguments.Count == 2)
			{
				return treeBuilder.MethodCall(function, firstArgument, visitor.Visit(arguments[1]).AsExpression());
			}

			return treeBuilder.MethodCall(function, firstArgument);
		}
	}
}
