using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class CollectionPropertyExpression : PropertyExpression
	{
		public CollectionPropertyExpression(string name, System.Type type, Expression source, IType nhibernateType)
			: base(name,NHExpressionType.CollectionProperty, type, source, nhibernateType)
		{
		}

		public CollectionType CollectionType { get { return base.NHibernateType as CollectionType; } }
	}
}