using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class MathGenerator : BaseHqlGeneratorForMethod
	{
		public MathGenerator()
		{
			SupportedMethods = new[]
								   {
									   ReflectionHelper.GetMethodDefinition(() => Math.Sin(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Cos(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Tan(default(double))),
									   
									   ReflectionHelper.GetMethodDefinition(() => Math.Sinh(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Cosh(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Tanh(default(double))),
									   
									   ReflectionHelper.GetMethodDefinition(() => Math.Asin(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Acos(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Atan(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Atan2(default(double), default(double))),

									   ReflectionHelper.GetMethodDefinition(() => Math.Sqrt(default(double))),
									   
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(float))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(long))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(int))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(short))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Abs(default(sbyte))),
									   
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(float))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(long))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(int))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(short))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Sign(default(sbyte))),
									   
									   ReflectionHelper.GetMethodDefinition(() => Math.Round(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Round(default(decimal), default(int))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Round(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Round(default(double), default(int))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Floor(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Floor(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Ceiling(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Ceiling(default(double))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Truncate(default(decimal))),
									   ReflectionHelper.GetMethodDefinition(() => Math.Truncate(default(double))),

									   ReflectionHelper.GetMethodDefinition(() => Math.Pow(default(double), default(double))),
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
