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
			HqlExpression result;

			string function = method.Name.ToLowerInvariant();

			HqlExpression[] expressions = 
				arguments
					.Select(x => visitor.Visit(x).AsExpression())
					.ToArray();

			switch (function)
			{
				case "add":
					result = treeBuilder.Add(expressions[0], expressions[1]);
					break;
				case "subtract":
					result = treeBuilder.Subtract(expressions[0], expressions[1]);
					break;
				case "divide":
					result = treeBuilder.Divide(expressions[0], expressions[1]);
					break;
				case "equals":
					return treeBuilder.Equality(expressions[0], expressions[1]);
				case "negate":
					result = treeBuilder.Negate(expressions[0]);
					break;
				case "compare":
					result = treeBuilder.MethodCall("sign", treeBuilder.Subtract(expressions[0], expressions[1]));
					break;
				case "multiply":
					result = treeBuilder.Multiply(expressions[0], expressions[1]);
					break;
				case "remainder":
					result = treeBuilder.MethodCall("mod", expressions[0], expressions[1]);
					break;
				case "round":
					HqlExpression numberOfDecimals = (arguments.Count == 2) ? expressions[1] : treeBuilder.Constant(0);
					result = treeBuilder.MethodCall("round", expressions[0], numberOfDecimals);
					break;
				default:
					result = treeBuilder.MethodCall(function, expressions[0]);
					break;
			}

			return treeBuilder.TransparentCast(result, typeof(decimal));
		}
	}
}
