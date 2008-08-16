using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// An order by expression is an expression that the result is ordered.
	/// This class holds reference to the actual expression(can be a projection, property, or an aggregate)
	/// </summary>
	public class OrderByFragment
	{
		public OrderByFragment(Expression orderBy,OrderByDirection direction)
		{
			this.OrderBy = orderBy;
			this.Direction = direction;
		}

		public Expression OrderBy { get; protected set; }
		public OrderByDirection Direction { get;protected set; }
	}
}
