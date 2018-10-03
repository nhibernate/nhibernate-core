using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public interface IHqlGeneratorForProperty
	{
		IEnumerable<MemberInfo> SupportedProperties { get; }

		HqlTreeNode BuildHql(
			MemberInfo member,
			Expression expression,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor);
	}

	// 6.0 TODO: merge into IHqlGeneratorForProperty
	public static class HqlGeneratorForPropertyExtensions
	{
		/// <summary>
		/// Should pre-evaluation be allowed for this property?
		/// </summary>
		/// <param name="generator">The property's HQL generator.</param>
		/// <param name="member">The property.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>
		/// <see langword="true" /> if the property should be evaluated before running the query whenever possible,
		/// <see langword="false" /> if it must always be translated to the equivalent HQL call.
		/// </returns>
		public static bool AllowPreEvaluation(
			this IHqlGeneratorForProperty generator,
			MemberInfo member,
			ISessionFactoryImplementor factory)
		{
			if (generator is IAllowPreEvaluationHqlGenerator allowPreEvalGenerator)
				return allowPreEvalGenerator.AllowPreEvaluation(member, factory);

			// By default, everything should be pre-evaluated whenever possible.
			return true;
		}
	}
}
