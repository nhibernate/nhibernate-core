using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class ComponentPropertyExpression : PropertyExpression
	{
		public ComponentPropertyExpression(string name, string[] columns, System.Type type, Expression source,
			IType nhibernateType)
			: base(name, type, source, nhibernateType)
		{
			this.Columns = columns;
		}

		public string[] Columns { get; protected set; }
		public ComponentType ComponentType { get { base.NHibernateType as ComponentType; } }
	}
}
