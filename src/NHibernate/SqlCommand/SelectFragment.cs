using System.Collections;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents part of an SQL <c>SELECT</c> clause
	/// </summary>
	public class SelectFragment
	{
		private string suffix;
		private IList columns = new ArrayList();
		private IList columnAliases = new ArrayList();
		private Dialect.Dialect dialect;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		public SelectFragment( Dialect.Dialect d )
		{
			this.dialect = d;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public SelectFragment SetSuffix( string suffix )
		{
			this.suffix = suffix;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public SelectFragment AddColumn( string columnName )
		{
			AddColumn( null, columnName );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnNames"></param>
		/// <returns></returns>
		public SelectFragment AddColumns( string[ ] columnNames )
		{
			for( int i = 0; i < columnNames.Length; i++ )
			{
				AddColumn( columnNames[ i ] );
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public SelectFragment AddColumn( string tableAlias, string columnName )
		{
			return AddColumn( tableAlias, columnName, columnName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnName"></param>
		/// <param name="columnAlias"></param>
		/// <returns></returns>
		public SelectFragment AddColumn( string tableAlias, string columnName, string columnAlias )
		{
			if( tableAlias == null || tableAlias.Length == 0 )
			{
				columns.Add( columnName );
			}
			else
			{
				columns.Add( tableAlias + StringHelper.Dot + columnName );
			}

			columnAliases.Add( columnAlias );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnNames"></param>
		/// <returns></returns>
		public SelectFragment AddColumns( string tableAlias, string[ ] columnNames )
		{
			for( int i = 0; i < columnNames.Length; i++ )
			{
				AddColumn( tableAlias, columnNames[ i ] );
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="columnNames"></param>
		/// <param name="columnAliases"></param>
		/// <returns></returns>
		public SelectFragment AddColumns( string tableAlias, string[ ] columnNames, string[ ] columnAliases )
		{
			for( int i = 0; i < columnNames.Length; i++ )
			{
				AddColumn( tableAlias, columnNames[ i ], columnAliases[ i ] );
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="formulas"></param>
		/// <param name="formulaAliases"></param>
		/// <returns></returns>
		public SelectFragment AddFormulas( string tableAlias, string[ ] formulas, string[ ] formulaAliases )
		{
			for( int i = 0; i < formulas.Length; i++ )
			{
				AddFormula( tableAlias, formulas[ i ], formulaAliases[ i ] );
			}

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableAlias"></param>
		/// <param name="formula"></param>
		/// <param name="formulaAlias"></param>
		/// <returns></returns>
		public SelectFragment AddFormula( string tableAlias, string formula, string formulaAlias )
		{
			AddColumn(
				null,
				StringHelper.Replace( formula, Template.PlaceHolder, tableAlias ),
				formulaAlias );

			return this;
		}

		/// <summary></summary>
		public SqlString ToSqlStringFragment()
		{
			// this preserves the way this existing method works now.
			return ToSqlStringFragment( true );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="includeLeadingComma"></param>
		/// <returns></returns>
		public SqlString ToSqlStringFragment( bool includeLeadingComma )
		{
			StringBuilder buf = new StringBuilder( columns.Count*10 );

			for( int i = 0; i < columns.Count; i++ )
			{
				string col = columns[ i ] as string;
				if( i > 0 || includeLeadingComma )
				{
					buf.Append( StringHelper.CommaSpace );
				}

				string columnAlias = columnAliases[ i ] as string;
				buf.Append( col )
					.Append( " as " )
					.Append( new Alias( suffix ).ToAliasString( columnAlias, dialect ) );
			}
			return new SqlString( buf.ToString() );
		}
	}
}