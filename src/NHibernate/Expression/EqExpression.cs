using System;

namespace NHibernate.Expression {
	
	public class EqExpression : SimpleExpression {

		internal EqExpression(string propertyName, object value) : base (propertyName, value) {
		}

		protected override string Op {
			get { return "="; }
		}
	}
}
