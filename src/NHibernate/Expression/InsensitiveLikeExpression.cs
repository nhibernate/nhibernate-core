using System;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint
	/// that is <b>not</b> case sensitive.
	/// </summary>
	//TODO:H2.0.3 renamed this to ILikeExpression
	[Serializable]
	public class InsensitiveLikeExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object _value;

		/// <summary>
		/// Initialize a new instance of the <see cref="InsensitiveLikeExpression" /> 
		/// class for a named Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public InsensitiveLikeExpression( string propertyName, object value )
		{
			_propertyName = propertyName;
			_value = value;
		}

		public InsensitiveLikeExpression( string propertyName, string value, MatchMode matchMode )
			: this( propertyName, matchMode.ToMatchString( value ) )
		{
		}

		public override SqlString ToSqlString( ICriteria criteria, ICriteriaQuery criteriaQuery )
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			string[ ] columnNames = criteriaQuery.GetColumnsUsingProjection( criteria, _propertyName );

			if( columnNames.Length != 1 )
			{
				throw new HibernateException( "insensitive like may only be used with single-column properties" );
			}

			if( criteriaQuery.Factory.Dialect is PostgreSQLDialect )
			{
				sqlBuilder.Add( columnNames[ 0 ] );
				sqlBuilder.Add( " ilike " );
			}
			else
			{
				sqlBuilder.Add( criteriaQuery.Factory.Dialect.LowercaseFunction )
					.Add( "(" )
					.Add( columnNames[ 0 ] )
					.Add( ")" )
					.Add( " like " );
			}

			sqlBuilder.AddParameter();

			return sqlBuilder.ToSqlString();
		}

		public override TypedValue[ ] GetTypedValues( ICriteria criteria, ICriteriaQuery criteriaQuery )
		{
			return new TypedValue[ ]
				{
					criteriaQuery.GetTypedValue( criteria, _propertyName, _value.ToString().ToLower() )
				};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " ilike " + _value;
		}
	}
}