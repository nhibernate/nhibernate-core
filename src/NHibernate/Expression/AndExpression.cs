using System;

namespace NHibernate.Expression {
	
	public class AndExpression : LogicalExpression {

		internal AndExpression(Expression lhs, Expression rhs) : base (lhs,rhs) {
		}

		protected override string Op {
			get { return "and"; }
		}
	}
}