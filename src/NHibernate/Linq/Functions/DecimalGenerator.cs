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
				ReflectHelper.GetMethodDefinition(() => decimal.Subtract(default(decimal), default(decimal)))
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			string function = method.Name.ToLowerInvariant();

			HqlExpression[] expressions = arguments.Select(x => visitor.Visit(x).AsExpression()).ToArray();

			if (function == "remainder")
			{
				function = "mod_decimal";
			}

			switch (function)
			{
				case "add":
					return treeBuilder.Add(expressions[0], expressions[1]);
				case "subtract":
					return treeBuilder.Subtract(expressions[0], expressions[1]);
				case "divide":
					return treeBuilder.Divide(expressions[0], expressions[1]);
				case "equals":
					return treeBuilder.Equality(expressions[0], expressions[1]);
				case "negate":
					return treeBuilder.Negate(expressions[0]);
				case "compare":
					return treeBuilder.MethodCall("sign", treeBuilder.Subtract(expressions[0], expressions[1]));
				case "multiply":
					return treeBuilder.Multiply(expressions[0], expressions[1]);
				case "round":
					HqlExpression numberOfDecimals = (arguments.Count == 2) ? expressions[1] : treeBuilder.Constant(0);
					return treeBuilder.MethodCall("round", expressions[0], numberOfDecimals);
			}

			if (arguments.Count == 2)
			{
				return treeBuilder.MethodCall(function, expressions[0], expressions[1]);
			}

			return treeBuilder.MethodCall(function, expressions[0]);
		}
	}
}
