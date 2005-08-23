using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Summary description for SqlSimpleSelectBuilder.
	/// </summary>
	public class SqlSimpleSelectBuilder : SqlBaseBuilder, ISqlStringBuilder
	{
		private string tableName;

		private IList columnNames = new ArrayList(); // name of the column
		private IDictionary aliases = new Hashtable(); //key=column Name, value=column Alias

		private int versionFragmentIndex = -1;  // not used !?!
		private int identityFragmentIndex = -1; // not used !?!

		private IList whereStrings = new ArrayList();

		//these can be plain strings because a forUpdate and orderBy will have
		// no parameters so using a SqlString will only complicate matters - or 
		// maybe simplify because any Sql will be contained in a known object type...
		private string forUpdateFragment;
		private string orderBy;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public SqlSimpleSelectBuilder( ISessionFactoryImplementor factory ) : base( factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public SqlSimpleSelectBuilder SetTableName( string tableName )
		{
			this.tableName = tableName;
			return this;
		}


		/// <summary>
		/// Adds a columnName to the SELECT fragment.
		/// </summary>
		/// <param name="columnName">The name of the column to add.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumn( string columnName )
		{
			columnNames.Add( columnName );
			return this;
		}

		/// <summary>
		/// Adds a columnName and its Alias to the SELECT fragment.
		/// </summary>
		/// <param name="columnName">The name of the column to add.</param>
		/// <param name="alias">The alias to use for the column</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumn( string columnName, string alias )
		{
			columnNames.Add( columnName );
			aliases[ columnName ] = alias;
			return this;
		}

		/// <summary>
		/// Adds an array of columnNames to the SELECT fragment.
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumns( string[ ] columnNames )
		{
			for( int i = 0; i < columnNames.Length; i++ )
			{
				AddColumn( columnNames[ i ] );
			}
			return this;
		}

		/// <summary>
		/// Adds an array of columnNames with their Aliases to the SELECT fragment.
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="aliases">The aliases to use for the columns</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddColumns( string[ ] columnNames, string[ ] aliases )
		{
			for( int i = 0; i < columnNames.Length; i++ )
			{
				AddColumn( columnNames[ i ], aliases[ i ] );
			}
			return this;
		}

		/// <summary>
		/// Gets the Alias that should be used for the column
		/// </summary>
		/// <param name="columnName">The name of the column to get the Alias for.</param>
		/// <returns>The Alias if one exists, null otherwise</returns>
		public string GetAlias( string columnName )
		{
			return ( string ) aliases[ columnName ];
		}

		/// <summary>
		/// Sets the IdentityColumn for the <c>SELECT</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="identityType">The IType of the Identity Property.</param>
		/// <returns>The SqlSimpleSelectBuilder.</returns>
		public SqlSimpleSelectBuilder SetIdentityColumn( string[ ] columnNames, IType identityType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Mapping, columnNames, identityType );

			identityFragmentIndex = whereStrings.Add( ToWhereString( columnNames, parameters ) );

			return this;
		}

		/// <summary>
		/// Sets the VersionColumn for the <c>SELECT</c> sql to use.
		/// </summary>
		/// <param name="columnNames">An array of the column names for the Property</param>
		/// <param name="versionType">The IVersionType of the Version Property.</param>
		/// <returns>The SqlSimpleSelectBuilder.</returns>
		public SqlSimpleSelectBuilder SetVersionColumn( string[ ] columnNames, IVersionType versionType )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Mapping, columnNames, versionType );

			versionFragmentIndex = whereStrings.Add( ToWhereString( columnNames, parameters ) );

			return this;
		}

		/// <summary>
		/// Sets the For Update Fragment to the Select Command
		/// </summary>
		/// <param name="fragment">The fragment to set.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder SetForUpdateFragment( string fragment )
		{
			this.forUpdateFragment = fragment;
			return this;
		}

		/// <summary>
		/// Set the Order By fragment of the Select Command
		/// </summary>
		/// <param name="orderBy">The OrderBy fragment.  It should include the SQL "ORDER BY"</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder SetOrderBy( string orderBy )
		{
			this.orderBy = orderBy;
			return this;
		}

		/// <summary>
		/// Adds the columns for the Type to the WhereFragment
		/// </summary>
		/// <param name="columnNames">The names of the columns to add.</param>
		/// <param name="type">The IType of the property.</param>
		/// <param name="op">The operator to put between the column name and value.</param>
		/// <returns>The SqlSimpleSelectBuilder</returns>
		public SqlSimpleSelectBuilder AddWhereFragment( string[ ] columnNames, IType type, string op )
		{
			Parameter[ ] parameters = Parameter.GenerateParameters( Mapping, columnNames, type );

			whereStrings.Add( ToWhereString( columnNames, parameters, op ) );

			return this;
		}

		#region ISqlStringBuilder Members

		/// <summary></summary>
		public SqlString ToSqlString()
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			bool commaNeeded = false;


			sqlBuilder.Add( "SELECT " );

			for( int i = 0; i < columnNames.Count; i++ )
			{
				string column = ( string ) columnNames[ i ];
				string alias = GetAlias( column );

				if( commaNeeded )
				{
					sqlBuilder.Add( StringHelper.CommaSpace );
				}

				sqlBuilder.Add( column );
				if( alias != null && !alias.Equals( column ) )
				{
					sqlBuilder.Add( " AS " )
						.Add( alias );
				}

				commaNeeded = true;
			}


			sqlBuilder.Add( " FROM " )
				.Add( tableName );

			sqlBuilder.Add( " WHERE " );

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

			if( forUpdateFragment != null )
			{
				sqlBuilder.Add( " " )
					.Add( forUpdateFragment )
					.Add( " " );
			}

			if( orderBy != null )
			{
				sqlBuilder.Add( orderBy );
			}

			return sqlBuilder.ToSqlString();
		}

		#endregion
	}
}