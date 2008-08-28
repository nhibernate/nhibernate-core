using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// Base expression for transformed nodes.
	/// </summary>
	public abstract class NHExpression:Expression
	{
		/// <summary>
		/// Base class for the modified nodes.
		/// </summary>
		/// <param name="nodeType"></param>
		/// <param name="type"></param>
		protected NHExpression(NHExpressionType nodeType, System.Type type)
			: base((ExpressionType)nodeType, type)
		{
			
		}
	}
}
