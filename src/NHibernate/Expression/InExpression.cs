using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that constrains the property 
	/// to a specified list of values.
	/// </summary>
	/// <remarks>
	/// InExpression - should only be used with a Single Value column - no multicolumn properties...
	/// </remarks>
	public class InExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object[ ] _values;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="values"></param>
		internal InExpression( string propertyName, object[ ] values )
		{
			_propertyName = propertyName;
			_values = values;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses)
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			IType propertyType = ( ( IQueryable ) factory.GetPersister( persistentClass ) ).GetPropertyType( _propertyName );
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );
			// don't need to worry about aliasing or aliasClassing for parameter column names
			string[ ] paramColumnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName );

			if( columnNames.Length != 1 )
			{
				throw new HibernateException( "InExpression may only be used with single-column properties" );
			}

			// each value should have its own parameter
			Parameter[ ] parameters = new Parameter[_values.Length];

			for( int i = 0; i < _values.Length; i++ )
			{
				// we can hardcode 0 because this will only be for a single column
				string paramInColumnNames = paramColumnNames[ 0 ] + StringHelper.Underscore + i;
				parameters[ i ] = Parameter.GenerateParameters( factory, alias, new string[ ] {paramInColumnNames}, propertyType )[ 0 ];
			}

			sqlBuilder.Add( columnNames[ 0 ] )
				.Add( " in (" );

			bool commaNeeded = false;
			foreach( Parameter parameter in parameters )
			{
				if( commaNeeded )
				{
					sqlBuilder.Add( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				sqlBuilder.Add( parameter );

			}

			sqlBuilder.Add( ")" );

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
			//TODO: h2.1 synch: fix this up quite a bit
			TypedValue[ ] tvs = new TypedValue[_values.Length];
			for( int i = 0; i < tvs.Length; i++ )
			{
				tvs[ i ] = AbstractCriterion.GetTypedValue( sessionFactory, persistentClass, _propertyName, _values[ i ], aliasClasses );
			}
			return tvs;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " in (" + StringHelper.ToString( _values ) + ')';
		}

	}
}