using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class EqualsGenerator : BaseHqlGeneratorForMethod
	{
		public EqualsGenerator()
		{
			SupportedMethods = new[]
				{
					ReflectHelper.GetMethodDefinition(() => string.Equals(default(string), default(string))),
					ReflectHelper.GetMethodDefinition<string>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<char>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<sbyte>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<byte>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<short>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<ushort>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<int>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<uint>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<long>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<ulong>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<float>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<double>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<decimal>(x => x.Equals(x)),

					ReflectHelper.GetMethodDefinition<Guid>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<DateTime>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<DateTimeOffset>(x => x.Equals(x)),
					ReflectHelper.GetMethodDefinition<bool>(x => x.Equals(default(bool)))
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