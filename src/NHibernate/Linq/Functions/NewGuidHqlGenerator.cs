using System;
using System.Collections.Generic;
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
		private readonly Dictionary<MethodInfo, string> _hqlFunctions = new Dictionary<MethodInfo, string>()
		{
			{ ReflectHelper.FastGetMethod(Guid.NewGuid), "new_uuid" },
#if NET9_0_OR_GREATER
			{ ReflectHelper.FastGetMethod(Guid.CreateVersion7), "new_uuid_v7" },
#endif
		};

		public NewGuidHqlGenerator()
		{
			SupportedMethods = _hqlFunctions.Keys;
		}

		public override HqlTreeNode BuildHql(
			MethodInfo method,
			Expression targetObject,
			ReadOnlyCollection<Expression> arguments,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall(_hqlFunctions[method]);
		}

		public bool AllowPreEvaluation(MemberInfo member, ISessionFactoryImplementor factory)
		{
			if (member is not MethodInfo method
				|| !_hqlFunctions.TryGetValue(method, out var functionName)
				|| factory.Dialect.Functions.ContainsKey(functionName))
				return false;

			if (factory.Settings.LinqToHqlFallbackOnPreEvaluation)
				return true;

			throw new QueryException(
				$"Cannot translate {member.DeclaringType.Name}.{member.Name}: {functionName} is " +
				$"not supported by {factory.Dialect}. Either enable the fallback on pre-evaluation " +
				$"({Environment.LinqToHqlFallbackOnPreEvaluation}) or evaluate {member.Name} " +
				"outside of the query.");
		}

		public bool IgnoreInstance(MemberInfo member)
		{
			// There is only a static method
			return true;
		}
	}
}
