using System.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A class that builds an <c>DELETE</c> sql statement.
	/// </summary>
	public class SqlDeleteBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( SqlDeleteBuilder ) );
		private string tableName;

		private int versionFragmentIndex = -1;  // not used !?!
		private int identityFragmentIndex = -1; // not used !?!

		private IList whereStrings = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public SqlDeleteBuilder( ISessionFactoryImplementor factory ) : base( factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public SqlDeleteBuilder SetTableName( string tableName )
		{
			this.tableName = tableName;
			return this;
		}


		/// <summary>
		/// Sets the IdentityColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetIdentityColumn( string[ ] columnNames, IType identityType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, identityType );

			identityFragmentIndex = whereStrings.Add( ToWhereString( columnNames, parameters ) );

			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>DELETE</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlDeleteBuilder.</returns>
		public SqlDeleteBuilder SetVersionColumn( string[ ] columnNames, IVersionType versionType )
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
		/// <returns>The SqlDeleteBuilder</returns>
		public SqlDeleteBuilder AddWhereFragment( string[ ] columnNames, IType type, string op )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Factory, columnNames, type );
			whereStrings.Add( ToWhereString( columnNames, parameters, op ) );

			return this;
		}

		/// <summary>
		/// Adds a string to the WhereFragement
		/// </summary>
		/// <param name="whereSql">A well formed sql statement with no parameters.</param>
		/// <returns>The SqlDeleteBuilder</returns>
		public SqlDeleteBuilder AddWhereFragment( string whereSql )
		{
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
			// will for sure have 3 parts and then each item in the WhereStrings
			int initialCapacity = 3;

			// add an "AND" for each whereString except the first one.
			initialCapacity += ( whereStrings.Count - 1 );

			for( int i = 0; i < whereStrings.Count; i++ )
			{
				initialCapacity += ( ( SqlString ) whereStrings[ i ] ).Count;
			}

			SqlStringBuilder sqlBuilder = new SqlStringBuilder( initialCapacity + 2 );

			sqlBuilder.Add( "DELETE FROM " )
				.Add( tableName )
				.Add( " WHERE " );

			if( whereStrings.Count > 1 )
			{
				sqlBuilder.Add(
					( SqlString[ ] ) ( ( ArrayList ) whereStrings ).ToArray( typeof( SqlString ) ),
					null, "AND", null, false );
			}
			else
			{
				sqlBuilder.Add( ( SqlString ) whereStrings[ 0 ], null, null, null, false );
			}

			if( log.IsDebugEnabled )
			{
				if( initialCapacity < sqlBuilder.Count )
				{
					log.Debug(
						"The initial capacity was set too low at: " + initialCapacity + " for the DeleteSqlBuilder " +
							"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName );
				}
				else if( initialCapacity > 16 && ( ( float ) initialCapacity/sqlBuilder.Count ) > 1.2 )
				{
					log.Debug(
						"The initial capacity was set too high at: " + initialCapacity + " for the DeleteSqlBuilder " +
							"that needed a capacity of: " + sqlBuilder.Count + " for the table " + tableName );
				}
			}
			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}