using NHibernate.Util;

namespace NHibernate.Hql
{
	/// <summary> 
	/// Parses the ORDER BY clause of a query
	/// </summary>
	public class OrderByParser : IParser
	{
		// This uses a PathExpressionParser but notice that compound paths are not valid,
		// only bare names and simple paths:

		// SELECT p FROM p IN CLASS eg.Person ORDER BY p.Name, p.Address, p

		// The reason for this is SQL doesn't let you sort by an expression you are
		// not returning in the result set.

		private PathExpressionParser pathExpressionParser = new PathExpressionParser();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		public void Token( string token, QueryTranslator q )
		{
			if( q.IsName( StringHelper.Root( token ) ) )
			{
				ParserHelper.Parse( pathExpressionParser, q.Unalias( token ), ParserHelper.PathSeparators, q );
				q.AppendOrderByToken( pathExpressionParser.WhereColumn );
				pathExpressionParser.AddAssociation( q );
			}
			else if ( token.StartsWith( ParserHelper.HqlVariablePrefix ) )
			{
				q.AddNamedParameter( token.Substring( 1 ) );
				// this is only a temporary parameter to help with the parsing of hql - 
				// when the type becomes known then this will be converted to its real
				// parameter type.
				//AppendToken( q, new SqlString( new object[ ] {new Parameter( StringHelper.SqlParameter )} ) );
			}
			else
			{
				q.AppendOrderByToken( token );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void Start( QueryTranslator q )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void End( QueryTranslator q )
		{
		}

		/// <summary></summary>
		public OrderByParser()
		{
			pathExpressionParser.UseThetaStyleJoin = true;
		}
	}
}