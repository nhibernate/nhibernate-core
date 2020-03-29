using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Functions
{
	public class NewGuidHqlGenerator : BaseHqlGeneratorForMethod, IAllowPreEvaluationHqlGenerator
	{
		public NewGuidHqlGenerator()
		{
			SupportedMethods = new[]
			{
				ReflectHelper.GetMethod(() => Guid.NewGuid())
			};
		}

		public override HqlTreeNode BuildHql(
			MethodInfo method,
			Expression targetObject,
			ReadOnlyCollection<Expression> arguments,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall("new_uuid");
		}

		public bool AllowPreEvaluation(MemberInfo member, ISessionFactoryImplementor factory)
		{
			if (factory.Dialect.Functions.ContainsKey("new_uuid"))
				return false;

			if (factory.Settings.LinqToHqlFallbackOnPreEvaluation)
				return true;

			throw new QueryException(
				"Cannot translate NewGuid: new_uuid is " +
				$"not supported by {factory.Dialect}. Either enable the fallback on pre-evaluation " +
				$"({Environment.LinqToHqlFallbackOnPreEvaluation}) or evaluate NewGuid " +
				"outside of the query.");
		}

		public bool IgnoreInstance(MemberInfo member)
		{
			// There is only a static method
			return true;
		}
	}
}
