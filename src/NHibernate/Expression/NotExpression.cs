using System;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression 
{
	/// <summary>
	/// Negates another expression.
	/// </summary>
	public class NotExpression : Expression 
	{
		private Expression expression;

		internal NotExpression(Expression expression) 
		{
			this.expression = expression;
		}

		public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias) 
		{
			//TODO: set default capacity
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add("not ");
			builder.Add(expression.ToSqlString(factory, persistentClass, alias));

			return builder.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass) 
		{
			return expression.GetTypedValues(sessionFactory, persistentClass);
		}

		public override string ToString() {
			return "not " + expression.ToString();
		}
	}
}