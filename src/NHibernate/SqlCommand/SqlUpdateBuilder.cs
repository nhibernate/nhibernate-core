using System.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>UPDATE</c> sql statement.
	/// </summary>
	public class SqlUpdateBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( SqlUpdateBuilder ) );

		private string tableName;

		private IList columnNames = new ArrayList(); // name of the column
		private IList columnValues = new ArrayList(); //string or a Parameter

		private int identityFragmentIndex = -1; // not used !?!
		private int versionFragmentIndex = -1;  // not used !?!

		private IList whereStrings = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public SqlUpdateBuilder( ISessionFactoryImplementor factory ) : base( factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public SqlUpdateBuilder SetTableName( string tableName )
		{
			this.tableName = tableName;
			return this;
		}


		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">The value to set for the column.</param>
		/// <param name="literalType">The NHibernateType to use to convert the value to a sql string.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn( string columnName, object val, ILiteralType literalType )
		{
			return AddColumn( columnName, literalType.ObjectToSQLString( val ) );
		}


		/// <summary>
		/// Add a column with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The name of the Column to add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumn( string columnName, string val )
		{
			columnNames.Add( columnName );
			columnValues.Add( val );

			return this;
		}

		/// <summary>
		/// Adds columns with a specific value to the INSERT sql
		/// </summary>
		/// <param name="columnName">The names of the Column sto add.</param>
		/// <param name="val">A valid sql string to set as the value of the column.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns( string[ ] columnName, string val )
		{
			for( int i = 0; i < columnName.Length; i++ )
			{
				columnNames.Add( columnName[ i ] );
				columnValues.Add( val );
			}

			return this;
		}

		/// <summary>
		/// Adds the Property's columns to the UPDATE sql
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="propertyType">The IType of the property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder AddColumns( string[ ] columnNames, IType propertyType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, propertyType );

			for( int i = 0; i < columnNames.Length; i++ )
			{
				this.columnNames.Add( columnNames[ i ] );
				columnValues.Add( parameters[ i ] );
			}

			return this;
		}

		/// <summary>
		/// Sets the IdentityColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetIdentityColumn( string[ ] columnNames, IType identityType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, identityType );

			identityFragmentIndex = whereStrings.Add( ToWhereString( columnNames, parameters ) );

			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>UPDATE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlUpdateBuilder.</returns>
		public SqlUpdateBuilder SetVersionColumn( string[ ] columnNames, IVersionType versionType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, versionType );

			versionFragmentIndex = whereStrings.Add( ToWhereString( columnNames, parameters ) );

			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlUpdateBuilder</returns>
		public SqlUpdateBuilder AddWhereFragment( string[ ] columnNames, IType type, string op )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, type );
			whereStrings.Add( ToWhereString( columnNames, parameters, op ) );

			return this;
		}

		/// <summary>
		/// Adds a string to the WhereFragement
		/// </summary>
		/// <param name="whereSql">A well formed sql string with no parameters.</param>
		/// <returns>The SqlUpdateBuilder</returns>
		public SqlUpdateBuilder AddWhereFragment( string whereSql )
		{
			// Don't add empty condition's - we get extra ANDs
			if ( whereSql != null && whereSql.Length > 0 )
			{
				whereStrings.Add( new SqlString( whereSql ) );
			}
			return this;
		}

		#region ISqlStringBuilder Members

		/// <summary></summary>
		public SqlString ToSqlString()
		{
			// 3 = "UPDATE", tableName, "SET"
			int initialCapacity = 3;

			// will have a comma for all but the first column, and then for each column
			// will have a name, " = ", value so mulitply by 3
			if( columnNames.Count > 0 )
			{
				initialCapacity += ( columnNames.Count - 1 ) + ( columnNames.Count*3 );
			}
			// 1 = "WHERE" 
			initialCapacity++;

			// the "AND" before all but the first whereString
			if( whereStrings.Count > 0 )
			{
				initialCapacity += ( whereStrings.Count - 1 );
				for( int i = 0; i < whereStrings.Count; i++ )
				{
					initialCapacity += ( ( SqlString ) whereStrings[ i ] ).Count;
				}
			}

			SqlStringBuilder sqlBuilder = new SqlStringBuilder( initialCapacity + 2 );

			bool commaNeeded = false;
			bool andNeeded = false;


			sqlBuilder.Add( "UPDATE " )
				.Add( tableName )
				.Add( " SET " );

			for( int i = 0; i < columnNames.Count; i++ )
			{
				if( commaNeeded )
				{
					sqlBuilder.Add( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				string columnName = ( string ) columnNames[ i ];
				object columnValue = columnValues[ i ];

				sqlBuilder.Add( columnName )
					.Add( " = " );

				Parameter param = columnValue as Parameter;
				if( param != null )
				{
					sqlBuilder.Add( param );
				}
				else
				{
					sqlBuilder.Add( ( string ) columnValue );
				}

			}

			sqlBuilder.Add( " WHERE " );

			foreach( SqlString whereString in whereStrings )
			{
				if( andNeeded )
				{
					sqlBuilder.Add( " AND " );
				}
				andNeeded = true;

				sqlBuilder.Add( whereString, null, null, null, false );

			}

			if( log.IsDebugEnabled )
			{
				if( initialCapacity < sqlBuilder.Count )
				{
					log.Debug(
						"The initial capacity was set too low at: " + initialCapacity + " for the UpdateSqlBuilder " +
							"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName );
				}
				else if( initialCapacity > 16 && ( ( float ) initialCapacity/sqlBuilder.Count ) > 1.2 )
				{
					log.Debug(
						"The initial capacity was set too high at: " + initialCapacity + " for the UpdateSqlBuilder " +
							"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName );
				}
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}