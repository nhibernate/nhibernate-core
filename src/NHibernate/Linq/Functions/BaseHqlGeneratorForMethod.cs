﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public abstract class BaseHqlGeneratorForMethod : IHqlGeneratorForMethod, IHqlGeneratorForMethodExtended
	{
		public IEnumerable<MethodInfo> SupportedMethods { get; protected set; }

		public abstract HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor);

		public virtual bool AllowsNullableReturnType(MethodInfo method) => true;

		/// <inheritdoc />
		public virtual bool TryGetCollectionParameter(MethodCallExpression expression, out ConstantExpression collectionParameter)
		{
			collectionParameter = null;
			return false;
		}
	}
}
