using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.GH9999
{
	public class DateTimeDayOfYearPropertyHqlGenerator : BaseHqlGeneratorForProperty
	{
		public DateTimeDayOfYearPropertyHqlGenerator()
		{
			SupportedProperties = new[]
			{
				ReflectHelper.GetProperty((DateTime x) => x.DayOfYear)
			};
		}

		public override HqlTreeNode BuildHql(
			MemberInfo member,
			Expression expression,
			HqlTreeBuilder treeBuilder,
			IHqlExpressionVisitor visitor
		)
		{
			return treeBuilder.MethodCall(
				nameof(DateTime.DayOfYear),
				visitor.Visit(expression)
					.AsExpression()
			);
		}
	}
}
