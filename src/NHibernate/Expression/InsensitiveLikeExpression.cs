using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint
	/// that is <b>not</b> case sensitive.
	/// </summary>
	//TODO:H2.0.3 renamed this to ILikeExpression
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

			IType propertyType = AbstractCriterion.GetType( factory, persistentClass,_propertyName, aliasClasses );
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );
			Parameter[ ] parameters = Parameter.GenerateParameters( factory, columnNames, propertyType );

			if( factory.Dialect is PostgreSQLDialect )
			{
				sqlBuilder.Add( columnNames[ 0 ] );
				sqlBuilder.Add( " ilike " );
			}
			else
			{
				sqlBuilder.Add( factory.Dialect.LowercaseFunction )
					.Add( "(" )
					.Add( columnNames[ 0 ] )
					.Add( ")" )
					.Add( " like " );
			}

			sqlBuilder.Add( parameters[ 0 ] );

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
			return new TypedValue[ ] { AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, _value.ToString().ToLower(), aliasClasses )};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " ilike " + _value;
		}
	}
}