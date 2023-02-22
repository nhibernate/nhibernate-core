using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	/// <summary>
	/// Not using <see cref="EqualsGenerator"/> class as there is very special handling of enums in expressions and equality operator
	/// </summary>
	public class EnumEqualsGenerator : BaseHqlGeneratorForMethod
	{
		internal static HashSet<MethodInfo> Methods = new HashSet<MethodInfo>
		{
			ReflectHelper.GetMethodDefinition<Enum>(x => x.Equals(default(object))),
			ReflectHelper.GetMethodDefinition<IEquatable<Enum>>(x => x.Equals(default(Enum)))
		};

		public EnumEqualsGenerator()
		{
			SupportedMethods = Methods;
		}

		public override bool AllowsNullableReturnType(MethodInfo method) => false;

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			Expression lhs = arguments.Count == 1 ? targetObject : arguments[0];
			Expression rhs = arguments.Count == 1 ? arguments[0] : arguments[1];

			return treeBuilder.Equality(visitor.Visit(lhs).AsExpression(), visitor.Visit(rhs).AsExpression());
		}
	}
}
