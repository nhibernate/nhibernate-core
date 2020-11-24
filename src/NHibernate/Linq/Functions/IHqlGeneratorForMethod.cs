using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
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

		/// <summary>
		/// Try getting a collection parameter from <see cref="MethodCallExpression"/>.
		/// </summary>
		/// <param name="expression">The method call expression.</param>
		/// <param name="collectionParameter">Output parameter for the retrieved collection parameter.</param>
		/// <returns>Whether collection parameter was retrieved.</returns>
		bool TryGetCollectionParameter(MethodCallExpression expression, out ConstantExpression collectionParameter);
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

		// 6.0 TODO: Remove
		public static bool TryGetCollectionParameters(
			this IHqlGeneratorForMethod generator,
			MethodCallExpression expression,
			out ConstantExpression collectionParameter)
		{
			if (generator is IHqlGeneratorForMethodExtended extendedGenerator)
			{
				return extendedGenerator.TryGetCollectionParameter(expression, out collectionParameter);
			}

			collectionParameter = null;
			return false;
		}

		// 6.0 TODO: merge into IHqlGeneratorForMethod
		/// <summary>
		/// Should pre-evaluation be allowed for this method?
		/// </summary>
		/// <param name="generator">The method's HQL generator.</param>
		/// <param name="member">The method.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>
		/// <see langword="true" /> if the method should be evaluated before running the query whenever possible,
		/// <see langword="false" /> if it must always be translated to the equivalent HQL call.
		/// </returns>
		public static bool AllowPreEvaluation(
			this IHqlGeneratorForMethod generator,
			MemberInfo member,
			ISessionFactoryImplementor factory)
		{
			if (generator is IAllowPreEvaluationHqlGenerator allowPreEvalGenerator)
				return allowPreEvalGenerator.AllowPreEvaluation(member, factory);

			// By default, everything should be pre-evaluated whenever possible.
			return true;
		}

		/// <summary>
		/// Should the instance holding the method be ignored?
		/// </summary>
		/// <param name="generator">The method's HQL generator.</param>
		/// <param name="member">The method.</param>
		/// <returns>
		/// <see langword="true" /> if the method translation does not depend on the instance to which it
		/// belongs, <see langword="false" /> otherwise.
		/// </returns>
		public static bool IgnoreInstance(this IHqlGeneratorForMethod generator, MemberInfo member)
		{
			if (generator is IAllowPreEvaluationHqlGenerator allowPreEvalGenerator)
				return allowPreEvalGenerator.IgnoreInstance(member);

			return false;
		}
	}
}
