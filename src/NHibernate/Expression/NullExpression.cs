using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents "null" constraint.
	/// </summary>
	public class NullExpression : AbstractCriterion
	{
		private readonly string _propertyName;

		private static readonly TypedValue[ ] NoValues = new TypedValue[0];

		/// <summary>
		/// Initialize a new instance of the <see cref="NotNullExpression" /> class for a named
		/// Property that should be null.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		public NullExpression( string propertyName )
		{
			_propertyName = propertyName;
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
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );

			for( int i = 0; i < columnNames.Length; i++ )
			{
				if( i > 0 )
				{
					sqlBuilder.Add( " AND " );
				}

				sqlBuilder.Add( columnNames[ i ] )
					.Add( " IS NULL" );

			}

			return sqlBuilder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <returns></returns>
		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			return NoValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " is null";
		}
	}
}