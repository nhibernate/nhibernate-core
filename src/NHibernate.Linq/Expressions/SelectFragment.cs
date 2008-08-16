using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	public class SelectFragment
	{
		public SelectFragment(string alias,SqlExpression expression)
		{
			this.Alias = alias;
			this.Expression = expression;
		}

		public string Alias { get; protected set; }
		public SqlExpression Expression { get; protected set; }
		public override string ToString()
		{
			return string.Format("({0} as {1})", Expression.ToString(),Alias);
		}
	}
}
