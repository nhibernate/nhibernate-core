using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    public interface IHqlGeneratorForMethod
    {
        IEnumerable<MethodInfo> SupportedMethods { get; }
        HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor);
    }

	// 6.0 TODO: Merge into IHqlGeneratorForMethod
	internal interface IHqlGeneratorForMethodExtended
	{
		bool AllowsNullableReturnType(MethodInfo method);
	}

	internal static class HqlGeneratorForMethodExtensions
	{
		// 6.0 TODO: Remove
		public static bool AllowsNullableReturnType(this IHqlGeneratorForMethod generator, MethodInfo method)
		{
			if (generator is IHqlGeneratorForMethodExtended extendedGenerator)
			{
				return extendedGenerator.AllowsNullableReturnType(method);
			}

			return true;
		}
	}
}
