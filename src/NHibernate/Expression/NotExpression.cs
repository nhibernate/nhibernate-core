using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// Negates another expression.
	/// </summary>
	public class NotExpression : Expression
	{
		private Expression expression;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="expression"></param>
		internal NotExpression( Expression expression )
		{
			this.expression = expression;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias )
		{
			//TODO: set default capacity
			SqlStringBuilder builder = new SqlStringBuilder();
			builder.Add( "not " );
			builder.Add( expression.ToSqlString( factory, persistentClass, alias ) );

			return builder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass )
		{
			return expression.GetTypedValues( sessionFactory, persistentClass );
		}

		/// <summary></summary>
		public override string ToString()
		{
			return "not " + expression.ToString();
		}
	}
}