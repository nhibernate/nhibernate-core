using System;

namespace NHibernate.Expression {
	
	public class OrExpression : LogicalExpression {

		internal OrExpression(Expression lhs, Expression rhs) : base (lhs,rhs) {
		}

		protected override string Op {
			get { return "or"; }
		}
	}
}
