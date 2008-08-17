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
		public SelectExpression(System.Type type, string alias, Expression from, Expression where)
			: base(SqlExpressionType.Select, type)
		{
			this.Where = where;
			this.From = from;
			this.Alias = alias;
		}

		public string Alias { get; set; }
		public Expression Where { get; protected set; }
		public Expression From { get; protected set; }
	}
}