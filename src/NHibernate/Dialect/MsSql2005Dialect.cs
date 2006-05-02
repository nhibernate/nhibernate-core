using System;
using System.Text;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A <see cref="Dialect"/> for <c>Microsoft Sql Server 2005</c>.
	/// </summary>
	public class MsSql2005Dialect : MsSql2000Dialect
	{
		/// <summary>
		/// Add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">The <see cref="SqlString"/> to base the limit query off of.</param>
		/// <param name="offset">Offset of the first row to be returned by the query (zero-based)</param>
		/// <param name="last">Maximum number of rows to be returned by the query</param>
		/// <returns>A new <see cref="SqlString"/> with the <c>LIMIT</c> clause applied.</returns>
		/// <remarks>
		/// The <c>LIMIT</c> SQL will look like
		/// <code>
		/// WITH query AS
		///     (SELECT TOP last ROW_NUMBER() OVER (ORDER BY orderby) as __hibernate_row_nr__, ... original_query)
		///  SELECT * 
		///    FROM query
		///    WHERE __hibernate_row_nr__ > offset
		///    ORDER BY __hibernate_row_nr__
		/// </code>
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			StringBuilder orderByStringBuilder = new StringBuilder();
			string distinctStr = string.Empty;

			foreach (object sqlPart in querySqlString.SqlParts )
			{
				string sqlPartString = sqlPart as string;
				if( sqlPartString!=null )
				{
					string loweredString = sqlPartString.ToLower();
					int orderByIndex = loweredString.IndexOf("order by");
					if( orderByIndex!=-1 )
					{
						// if we find a new "order by" then we need to ignore
						// the previous one since it was probably used for a subquery
						orderByStringBuilder = new StringBuilder();
						orderByStringBuilder.Append( sqlPartString.Substring( orderByIndex ) );
					}
					if( loweredString.TrimStart().StartsWith( "select" ) )
					{
						int index = 6;
						if( loweredString.StartsWith( "select distinct" ) )
						{
							distinctStr = "DISTINCT ";
							index = 15;
						}
						sqlPartString = sqlPartString.Substring( index );
					}

					pagingBuilder.Add( sqlPartString );
				}
				else
				{
					pagingBuilder.AddObject( sqlPart );
				}
			}

			string orderby = orderByStringBuilder.ToString();
			// if no ORDER BY is specified use fake ORDER BY field to avoid errors 
			if( orderby==null || orderby.Length==0 ) 
			{
				orderby = "ORDER BY CURRENT_TIMESTAMP";
			}

			string beginning =
				string.Format("WITH query AS (SELECT {0}TOP {1} ROW_NUMBER() OVER ({2}) as __hibernate_row_nr__, ",
							  distinctStr, last, orderby);
			string ending =
				string.Format(") SELECT * FROM query WHERE __hibernate_row_nr__ > {0} ORDER BY __hibernate_row_nr__",
							  offset);

			pagingBuilder.Insert( 0, beginning );
			pagingBuilder.Add( ending );

			return pagingBuilder.ToSqlString();
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionallity.
		/// </summary>
		/// <value><c>true</c></value>
		public override	bool SupportsLimit
		{
			get	{ return true; }
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionallity with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override	bool SupportsLimitOffset
		{
			get	{ return true; }
		}
	}
}
