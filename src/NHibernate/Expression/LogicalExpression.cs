using System;

using NHibernate.Engine;

namespace NHibernate.Expression {

	public abstract class LogicalExpression : Expression {

		protected Expression lhs;
		protected Expression rhs;

		internal LogicalExpression(Expression lhs, Expression rhs) {
			this.lhs = lhs;
			this.rhs = rhs;
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor	sessionFactory, System.Type persistentClass) {
			TypedValue[] lhstv = lhs.GetTypedValues(sessionFactory, persistentClass);
			TypedValue[] rhstv = rhs.GetTypedValues(sessionFactory, persistentClass);
			TypedValue[] result = new TypedValue[ lhstv.Length + rhstv.Length ];
			Array.Copy(lhstv, 0, result, 0, lhstv.Length);
			Array.Copy(rhstv, 0, result, lhstv.Length, rhstv.Length);
			return result;
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			return '(' + 
				lhs.ToSqlString(sessionFactory, persistentClass, alias) + 
				' ' + 
				Op + 
				' ' + 
				rhs.ToSqlString(sessionFactory, persistentClass, alias) +
				')';
		}

		protected abstract string Op { get; } //protected ???

		public override string ToString() {
			return lhs.ToString() + ' ' + Op + ' ' + rhs.ToString();
		}
	}
}