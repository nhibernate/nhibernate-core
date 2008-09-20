using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Linq.Expressions
{
	public abstract class AssociationPropertyExpression:PropertyExpression
	{
		public AssociationPropertyExpression(string name,NHExpressionType nodeType,System.Type type,
			Expression source,IType nhType):base(name,nodeType,type,source,nhType)
		{
			
		}

		public IAssociationType AssociationType { get { return base.NHibernateType as IAssociationType; } }
	}
}
