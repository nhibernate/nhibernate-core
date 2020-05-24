using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public abstract class BaseHqlGeneratorForMethod : IHqlGeneratorForMethod, IHqlGeneratorForMethodExtended
	{
		protected static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(BaseHqlGeneratorForMethod));

		public IEnumerable<MethodInfo> SupportedMethods { get; protected set; }

		public abstract HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor);

		public virtual bool AllowsNullableReturnType(MethodInfo method) => true;

		private protected static void LogIgnoredParameter(MethodInfo method, string paramType)
		{
			if (Log.IsWarnEnabled())
				Log.Warn("Method parameter of type '{0}' is ignored when converting to hql the following method: {1}", paramType, method);
		}

		private protected static void LogIgnoredStringComparisonParameter(MethodInfo actualMethod, MethodInfo methodWithStringComparison)
		{
			if (actualMethod == methodWithStringComparison)
				LogIgnoredParameter(actualMethod, nameof(StringComparison));
		}

		private protected bool LogIgnoredStringComparisonParameter(MethodInfo actualMethod, params MethodInfo[] methodsWithStringComparison)
		{
			if (!methodsWithStringComparison.Contains(actualMethod))
				return false;

			LogIgnoredParameter(actualMethod, nameof(StringComparison));
			return true;
		}
	}
}
