using System;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for NotExpression.
	/// </summary>
	public class NotExpression : Expression
	{
		private Expression expression;

		internal NotExpression(Expression expression) {
			this.expression = expression;
		}

		public override string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			return "not " + expression.ToSqlString(sessionFactory, persistentClass, alias);
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) {
			return expression.GetTypedValues(sessionFactory, persistentClass);
		}

		public override string ToString() {
			return "not " + expression.ToString();
		}
	}
}