using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class PropertyExpression:NHExpression
	{
		public PropertyExpression(string name, 
			System.Type type, Expression expression, IType nhibernateType)
			: this(name, type, expression,nhibernateType, NHExpressionType.Property) 
		{
	
        }



		protected PropertyExpression(string name, System.Type type,Expression expression, IType nhibernateType, NHExpressionType nodeType)
            : base(nodeType, type)
		{
			this.NHibernateType = nhibernateType;
			this.Expression = expression;
			this.Name = name;
		}

		public Expression Expression { get; protected set; }
		public IType NHibernateType { get; protected set; }
		public string Name { get; protected set; }

		public override string ToString()
		{
			return this.Expression.ToString() + "." + Name;
		}
	}
}
