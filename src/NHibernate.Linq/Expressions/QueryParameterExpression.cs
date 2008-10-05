using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	public class QueryParameterExpression:NHExpression
	{
		public QueryParameterExpression(object value, int ordinal, System.Type type)
			: base(NHExpressionType.Parameter, type)
		{
			this.Value = value;
			this.Ordinal = ordinal;
		}

		public object Value { get; protected set; }
		public int Ordinal { get; protected set; }
	}
}
