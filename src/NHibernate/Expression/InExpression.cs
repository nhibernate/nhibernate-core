using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
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
		public InExpression( string propertyName, object[ ] values )
		{
			_propertyName = propertyName;
			_values = values;
		}

		private static Parameter[ ] GenerateValueParameters( string prefix, SqlType sqlType, int count )
		{
			Parameter[ ] parameters = new Parameter[count];

			for( int i = 0; i < count; i++ )
			{
				string parameterName = StringHelper.Suffix( prefix, StringHelper.Underscore + i.ToString() );
				parameters[ i ] = new Parameter( parameterName, sqlType );
			}

			return parameters;
		}

		public override SqlString ToSqlString( ISessionFactoryImplementor factory, System.Type persistentClass, string alias, IDictionary aliasClasses )
		{
			if( _values.Length == 0 )
			{
				// "something in ()" is always false
				return new SqlString( "1=0" );
			}

			//TODO: add default capacity
			SqlStringBuilder result = new SqlStringBuilder();

			IType propertyType = AbstractCriterion.GetType( factory, persistentClass, _propertyName, aliasClasses );

			SqlType[ ] columnSqlTypes = propertyType.SqlTypes( factory );
			string[ ] columnNames = AbstractCriterion.GetColumns( factory, persistentClass, _propertyName, alias, aliasClasses );

			// Generate SqlString of the form:
			// columnName1 in (values) and columnName2 in (values) and ...

			for( int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++ )
			{
				string columnName = columnNames[ columnIndex ];
				SqlType columnSqlType = columnSqlTypes[ columnIndex ];

				if( columnIndex > 0 )
				{
					result.Add( " and " );
				}

				result
					.Add( columnName )
					.Add( " in (" );

				Parameter[ ] valueParameters = GenerateValueParameters( columnName, columnSqlType, _values.Length );
				for( int i = 0; i < valueParameters.Length; i++ )
				{
					if( i > 0 )
					{
						result.Add( StringHelper.CommaSpace );
					}
					result.Add( valueParameters[ i ] );
				}

				result.Add( ")" );
			}

			return result.ToSqlString();
		}

		public override TypedValue[ ] GetTypedValues( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, IDictionary aliasClasses )
		{
			ArrayList list = new ArrayList();
			IType type = GetType( sessionFactory, persistentClass, _propertyName, aliasClasses );

			if( type.IsComponentType )
			{
				IAbstractComponentType actype = ( IAbstractComponentType ) type;
				IType[ ] types = actype.Subtypes;

				for( int i = 0; i < types.Length; i++ )
				{
					for( int j = 0; j < _values.Length; j++ )
					{
						object subval = _values[ j ] == null ?
							null :
							actype.GetPropertyValues( _values[ j ] )[ i ];
						list.Add( new TypedValue( types[ i ], subval ) );
					}
				}
			}
			else
			{
				for( int j = 0; j < _values.Length; j++ )
				{
					list.Add( new TypedValue( type, _values[ j ] ) );
				}
			}

			return ( TypedValue[ ] ) list.ToArray( typeof( TypedValue ) );
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " in (" + StringHelper.ToString( _values ) + ')';
		}

	}
}