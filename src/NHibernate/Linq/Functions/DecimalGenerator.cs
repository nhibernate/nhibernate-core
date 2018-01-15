using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class DecimalGenerator : BaseHqlGeneratorForMethod
	{
		public DecimalGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethodDefinition(() => decimal.Add(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Ceiling(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Compare(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Divide(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Equals(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Floor(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Multiply(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Negate(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Remainder(default(decimal), default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal))),
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal), default(int))),
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal), default(MidpointRounding))),
				ReflectHelper.GetMethodDefinition(() => decimal.Round(default(decimal), default(int), default(MidpointRounding))),
				ReflectHelper.GetMethodDefinition(() => decimal.Subtract(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string function = method.Name.ToLowerInvariant();

			return treeBuilder.MethodCall(function, arguments.Select(x => visitor.Visit(x).AsExpression()));
		}
	}
}
