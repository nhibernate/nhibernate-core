using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class DateTimePropertiesHqlGenerator : BaseHqlGeneratorForProperty
	{
		public DateTimePropertiesHqlGenerator()
		{
			SupportedProperties = new[]
				{
					ReflectionHelper.GetProperty((DateTime x) => x.Year),
					ReflectionHelper.GetProperty((DateTime x) => x.Month),
					ReflectionHelper.GetProperty((DateTime x) => x.Day),
					ReflectionHelper.GetProperty((DateTime x) => x.Hour),
					ReflectionHelper.GetProperty((DateTime x) => x.Minute),
					ReflectionHelper.GetProperty((DateTime x) => x.Second),
					ReflectionHelper.GetProperty((DateTime x) => x.Date),
					
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Year),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Month),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Day),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Hour),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Minute),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Second),
					ReflectionHelper.GetProperty((DateTimeOffset x) => x.Date),
				};
		}

		public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall(member.Name.ToLowerInvariant(),
										  visitor.Visit(expression).AsExpression());
		}
	}
}