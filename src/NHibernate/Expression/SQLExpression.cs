using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that creates a SQLExpression
	/// </summary>
	/// <remarks>
	/// This allows for database specific Expressions at the cost of needing to 
	/// write a correct <see cref="SqlString"/>.
	/// </remarks>
	public class SQLExpression : AbstractCriterion
	{
		private readonly SqlString _sql;
		private readonly TypedValue[ ] _typedValues;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		public SQLExpression( SqlString sql, object[ ] values, IType[ ] types )
		{
			_sql = sql;
			_typedValues = new TypedValue[values.Length];
			for( int i = 0; i < _typedValues.Length; i++ )
			{
				_typedValues[ i ] = new TypedValue( types[ i ], values[ i ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses )
		{
			// TODO: h2.1 SYNCH - need to add an overload to Replace that takes 3 params
			return _sql.Replace( "$alias", alias );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return _typedValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _sql.ToString();
		}
	}
}