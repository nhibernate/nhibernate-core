using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// Creates a SQLExpression
	/// </summary>
	/// <remarks>
	/// This allows for database specific Expressions at the cost of needing to 
	/// write a correct <see cref="SqlString"/>.
	/// </remarks>
	public class SQLExpression : Expression
	{
		private readonly SqlString sql;
		private readonly TypedValue[ ] typedValues;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		internal SQLExpression( SqlString sql, object[ ] values, IType[ ] types )
		{
			this.sql = sql;
			typedValues = new TypedValue[values.Length];
			for( int i = 0; i < typedValues.Length; i++ )
			{
				typedValues[ i ] = new TypedValue( types[ i ], values[ i ] );
			}
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
			return sql.Replace( "$alias", alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass )
		{
			return typedValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return sql.ToString();
		}
	}
}