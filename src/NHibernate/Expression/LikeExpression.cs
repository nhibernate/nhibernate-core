using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for LikeExpression.
	/// </summary>
	public class LikeExpression: SimpleExpression {

		internal LikeExpression(string propertyName, object value) : base (propertyName, value) {}

		protected override string Op {
			get { return " like "; }
		}
	}
}
