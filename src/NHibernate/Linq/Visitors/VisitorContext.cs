using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Visitors
{
	public class VisitorContext
	{
		public VisitorContext(VisitorContext parent, Expression expression)
		{
			Parent = parent;
			Expression = expression;
		}

		public VisitorContext Parent { get; private set; }
		public Expression Expression { get; private set; }
	}
}
