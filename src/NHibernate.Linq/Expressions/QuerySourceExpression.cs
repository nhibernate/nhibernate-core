using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Metadata;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// A QuerySource expression is where we want to execute our query on, in RDBMS case it is a physical Table in general.
	/// May also be used in from clause of a select expression
	/// </summary>
	public class QuerySourceExpression:NHExpression
	{
		public QuerySourceExpression(IQueryable query)
			: base(NHExpressionType.QuerySource, query.GetType())
		{
			
			this.Query = query;
		}

		public IQueryable Query { get; protected set; }

		public override string ToString()
		{
			return string.Format("(({0})", Type.ToString());
		}
	}
}
