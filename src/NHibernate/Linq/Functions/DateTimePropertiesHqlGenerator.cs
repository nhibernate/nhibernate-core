using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Linq.Functions
{
	public class DateTimePropertiesHqlGenerator : BaseHqlGeneratorForProperty
	{
		public DateTimePropertiesHqlGenerator()
		{
			SupportedProperties = new[]
				{
					ReflectHelper.GetProperty((DateTime x) => x.Year),
					ReflectHelper.GetProperty((DateTime x) => x.Month),
					ReflectHelper.GetProperty((DateTime x) => x.Day),
					ReflectHelper.GetProperty((DateTime x) => x.Hour),
					ReflectHelper.GetProperty((DateTime x) => x.Minute),
					ReflectHelper.GetProperty((DateTime x) => x.Second),
					ReflectHelper.GetProperty((DateTime x) => x.Date),
					
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Year),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Month),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Day),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Hour),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Minute),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Second),
					ReflectHelper.GetProperty((DateTimeOffset x) => x.Date),
				};
		}

		public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall(member.Name.ToLowerInvariant(),
										  visitor.Visit(expression).AsExpression());
		}
	}
}