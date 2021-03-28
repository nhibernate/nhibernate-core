using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Linq.Functions
{
	public class DateTimeNowHqlGenerator : BaseHqlGeneratorForProperty, IAllowPreEvaluationHqlGenerator
	{
		private static readonly MemberInfo DateTimeNow = ReflectHelper.GetProperty(() => DateTime.Now);
		private static readonly MemberInfo DateTimeUtcNow = ReflectHelper.GetProperty(() => DateTime.UtcNow);
		private static readonly MemberInfo DateTimeToday = ReflectHelper.GetProperty(() => DateTime.Today);
		private static readonly MemberInfo DateTimeOffsetNow = ReflectHelper.GetProperty(() => DateTimeOffset.Now);
		private static readonly MemberInfo DateTimeOffsetUtcNow = ReflectHelper.GetProperty(() => DateTimeOffset.UtcNow);

		private readonly Dictionary<MemberInfo, string> _hqlFunctions = new Dictionary<MemberInfo, string>()
		{
			{ DateTimeNow, "current_timestamp" },
			{ DateTimeUtcNow, "current_utctimestamp" },
			// There is also sysdate, but it is troublesome: under some databases, "sys" prefixed functions return the
			// system time (time according to the server time zone) while "current" prefixed functions return the
			// session time (time according to the connection time zone), thus introducing a discrepancy with
			// current_timestamp.
			// Moreover sysdate is registered by default as a datetime, not as a date. (It could make sense for
			// Oracle, which returns a time part for dates, just dropping fractional seconds. But Oracle dialect
			// overrides it as a NHibernate date, without truncating it for SQL comparisons...)
			{ DateTimeToday, "current_date" },
			{ DateTimeOffsetNow, "current_timestamp_offset" },
			{ DateTimeOffsetUtcNow, "current_utctimestamp_offset" },
		};

		public DateTimeNowHqlGenerator()
		{
			SupportedProperties = new[]
			{
				DateTimeNow,
				DateTimeUtcNow,
				DateTimeToday,
				DateTimeOffsetNow,
				DateTimeOffsetUtcNow,
			};
		}

		public override HqlTreeNode BuildHql(
			MemberInfo member,
			Expression expression,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall(_hqlFunctions[member]);
		}

		public bool AllowPreEvaluation(MemberInfo member, ISessionFactoryImplementor factory)
		{
			var functionName = _hqlFunctions[member];
			if (factory.Dialect.Functions.ContainsKey(functionName))
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
			// They are all static properties
			return true;
		}
	}
}
