using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;

namespace NHibernate.Linq.Visitors
{
	public class SelectWhereJoinGatherer:NHibernateExpressionVisitor
	{
		public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression exp)
		{
			return base.Visit(exp);
		}
	}
}
