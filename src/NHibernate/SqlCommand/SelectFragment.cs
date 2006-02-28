using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
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
		private string[] usedAliases;
		private SqlString extraSelectList;

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
		public SelectFragment SetUsedAliases( string[] usedAliases )
		{
			this.usedAliases = usedAliases;
			return this;
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
				StringHelper.Replace( formula, Template.Placeholder, tableAlias ),
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
			HashedSet columnsUnique = new HashedSet();

			if ( usedAliases != null )
			{
				columnsUnique.AddAll( usedAliases );
			}

			bool found = false;
			for( int i = 0; i < columns.Count; i++ )
			{
				string col = columns[ i ] as string;
				string columnAlias = columnAliases[ i ] as string;

				if ( columnsUnique.Add( columnAlias ) )
				{
					if( found || includeLeadingComma )
					{
						buf.Append( StringHelper.CommaSpace );
					}

					buf.Append( col )
						.Append( " as " )
						.Append( new Alias( suffix ).ToAliasString( columnAlias, dialect ) );

					// Set the flag for the next time
					found = true;
				}
			}

			if( extraSelectList != null )
			{
				if( found || includeLeadingComma )
				{
					buf.Append( StringHelper.CommaSpace );
				}

				buf.Append( extraSelectList.ToString() );
			}

			return new SqlString( buf.ToString() );
		}

		public SelectFragment SetExtraSelectList( SqlString extraSelectList )
		{
			this.extraSelectList = extraSelectList;
			return this;
		}

		public SelectFragment SetExtraSelectList( CaseFragment caseFragment, string fragmentAlias )
		{
			SetExtraSelectList( caseFragment.SetReturnColumnName( fragmentAlias, suffix ).ToSqlStringFragment() );
			return this;
		}
	}
}