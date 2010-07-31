using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class DateTimePropertiesHqlGenerator : IHqlGeneratorForProperty
	{
		private readonly MemberInfo[] supportedProperties;

		public DateTimePropertiesHqlGenerator()
		{
			supportedProperties = new[]
			                      	{
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Year),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Month),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Day),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Hour),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Minute),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Second),
			                      		ReflectionHelper.GetProperty((DateTime x) => x.Date),
			                      	};
		}

		public IEnumerable<MemberInfo> SupportedProperties
		{
			get
			{
				return supportedProperties;
			}
		}

		public virtual HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			return treeBuilder.MethodCall(member.Name.ToLowerInvariant(),
			                              visitor.Visit(expression).AsExpression());
		}
	}
}