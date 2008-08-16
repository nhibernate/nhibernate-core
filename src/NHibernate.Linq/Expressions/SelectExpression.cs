using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// A select expression is executed against the database to retrieve data. 
	/// </summary>
	public class SelectExpression : SqlExpression
	{
		public SelectExpression(System.Type type, SqlExpression from, SqlExpression where, IList<OrderByFragment> orderBys)
			: base(SqlExpressionType.Select, type)
		{
			this.Where = where;
			this.OrderBys = orderBys;
			this.From = from;
		}

		public SqlExpression Where { get; protected set; }
		public SqlExpression From { get; protected set; }
		public IList<OrderByFragment> OrderBys { get; protected set; }
		public LimitFragment Limit { get; set; }
	}
}