using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class EqualsGenerator : BaseHqlGeneratorForMethod
	{
		public EqualsGenerator()
		{
			SupportedMethods = new[]
				{
					ReflectionHelper.GetMethodDefinition(() => string.Equals(default(string), default(string))),
					ReflectionHelper.GetMethodDefinition<string>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<char>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<sbyte>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<byte>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<short>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<ushort>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<int>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<uint>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<long>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<ulong>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<float>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<double>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<decimal>(x => x.Equals(x)),

					ReflectionHelper.GetMethodDefinition<Guid>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<DateTime>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<DateTimeOffset>(x => x.Equals(x)),
					ReflectionHelper.GetMethodDefinition<bool>(x => x.Equals(default(bool)))
				};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			Expression lhs = arguments.Count == 1 ? targetObject : arguments[0];
			Expression rhs = arguments.Count == 1 ? arguments[0] : arguments[1];

			return treeBuilder.Equality(
				visitor.Visit(lhs).ToArithmeticExpression(),
				visitor.Visit(rhs).ToArithmeticExpression());
		}
	}
}