using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public class OneToOnePropertyExpression:AssociationPropertyExpression
	{
		public OneToOnePropertyExpression(string name,string alias, System.Type type, Expression source, IType nhType)
			: base(name,alias, NHExpressionType.OneToOneProperty, type, source, nhType)
		{

		}

		public OneToOneType OneToOneType { get { return base.AssociationType as OneToOneType; } }
	}
}
