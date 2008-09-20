using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class SimplePropertyExpression:PropertyExpression
	{
		public SimplePropertyExpression(string name,string column, System.Type type, Expression source, IType nhibernateType)
			: base(name,NHExpressionType.SimpleProperty, type, source, nhibernateType)
		{
			this.Column = column;
		}

		public string Column { get; protected set; }
	}
}
