using System.Collections;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Hql
{
	/// <summary> 
	/// Parses the from clause of a hibernate query, looking for tables and
	/// aliases for the SQL query.
	/// </summary>
	public class FromParser : IParser
	{
		private PathExpressionParser peParser = new FromPathExpressionParser();
		private string entityName;
		private string alias;
		private bool afterIn;
		private bool afterAs;
		private bool afterClass;
		private bool expectingJoin;
		private bool expectingIn;
		private bool expectingAs;
		private bool afterJoinType;
		private bool afterFetch;
		private JoinType joinType = JoinType.None;

		private static IDictionary joinTypes = new Hashtable();

		/// <summary></summary>
		static FromParser()
		{
			joinTypes.Add( "left", JoinType.LeftOuterJoin );
			joinTypes.Add( "right", JoinType.RightOuterJoin );
			joinTypes.Add( "full", JoinType.FullJoin );
			joinTypes.Add( "inner", JoinType.InnerJoin );
		}

		public void Token( string token, QueryTranslator q )
		{
			// start by looking for HQL keywords....
			string lcToken = token.ToLower( System.Globalization.CultureInfo.InvariantCulture );
			if( lcToken.Equals( StringHelper.Comma ) )
			{
				if( !( expectingJoin | expectingAs ) )
				{
					throw new QueryException( "unexpected token: ," );
				}
				expectingJoin = false;
				expectingAs = false;
			}
			else if( lcToken.Equals( "join" ) )
			{
				if( !afterJoinType )
				{
					if( !( expectingJoin | expectingAs ) )
					{
						throw new QueryException( "unexpected token: join" );
					}
					// inner joins can be abbreviated to 'join'
					joinType = JoinType.InnerJoin;
					expectingJoin = false;
					expectingAs = false;
				}
				else
				{
					afterJoinType = false;
				}
			}
			else if( lcToken.Equals( "fetch" ) )
			{
				if( q.IsShallowQuery )
				{
					throw new QueryException( "fetch may not be used with scroll() or iterate()" );
				}
				if( joinType == JoinType.None )
				{
					throw new QueryException( "unexpected token: fetch" );
				}
				if( joinType == JoinType.FullJoin || joinType == JoinType.RightOuterJoin )
				{
					throw new QueryException( "fetch may only be used with inner join or left outer join" );
				}
				afterFetch = true;
			}
			else if( lcToken.Equals( "outer" ) )
			{
				// 'outer' is optional and is ignored)
				if( !afterJoinType || ( joinType != JoinType.LeftOuterJoin && joinType != JoinType.RightOuterJoin ) )
				{
					throw new QueryException( "unexpected token: outer" );
				}
			}
			else if( joinTypes.Contains( lcToken ) )
			{
				if( !( expectingJoin | expectingAs ) )
				{
					throw new QueryException( "unexpected token: " + token );
				}
				joinType = ( JoinType ) joinTypes[ lcToken ];
				afterJoinType = true;
				expectingJoin = false;
				expectingAs = false;
			}
			else if( lcToken.Equals( "class" ) )
			{
				if( !afterIn )
				{
					throw new QueryException( "unexpected token: class" );
				}
				if( joinType != JoinType.None )
				{
					throw new QueryException( "outer or full join must be followed by path expression" );
				}
				afterClass = true;
			}
			else if( lcToken.Equals( "in" ) )
			{
				if( !expectingIn )
				{
					throw new QueryException( "unexpected token: in" );
				}
				afterIn = true;
				expectingIn = false;
			}
			else if( lcToken.Equals( "as" ) )
			{
				if( !expectingAs )
				{
					throw new QueryException( "unexpected token: as" );
				}
				afterAs = true;
				expectingAs = false;
			}
			else
			{
				if( afterJoinType )
				{
					throw new QueryException( "join expected: " + token );
				}
				if( expectingJoin )
				{
					throw new QueryException( "unexpected token: " + token );
				}
				if( expectingIn )
				{
					throw new QueryException( "in expected: " + token );
				}

				// now anything that is not a HQL keyword

				if( afterAs || expectingAs )
				{
					// (AS is always optional, for consistentcy with SQL/OQL

					// process the "new" HQL stype where aliases are assigned
					// _after_ the class name or path expression ie using the
					// AS construction

					if( entityName != null )
					{
						q.SetAliasName( token, entityName );
					}
					else
					{
						throw new QueryException( "unexpected: as " + token );
					}
					afterAs = false;
					expectingJoin = true;
					expectingAs = false;
					entityName = null;
				}
				else if( afterIn )
				{
					// process the "old" HQL style where aliases appear _first
					// ie using the IN or IN CLASS constructions

					if( alias == null )
					{
						throw new QueryException( "alias not specified for: " + token );
					}

					if( joinType != JoinType.None )
					{
						throw new QueryException( "outer or full join must be followed by path expressions" );
					}

					if( afterClass )
					{
						// treat it as a classname
						IQueryable p = q.GetPersisterUsingImports( token );
						if( p == null )
						{
							throw new QueryException( "persister not found: " + token );
						}
						q.AddFromClass( alias, p );
					}
					else
					{
						// treat it as a path expression
						peParser.JoinType = JoinType.InnerJoin;
						peParser.UseThetaStyleJoin = true;
						ParserHelper.Parse( peParser, q.Unalias( token ), ParserHelper.PathSeparators, q );
						if( !peParser.IsCollectionValued )
						{
							throw new QueryException( "pathe expression did not resolve to collection: " + token );
						}
						string nm = peParser.AddFromCollection( q );
						q.SetAliasName( alias, nm );
					}

					alias = null;
					afterIn = false;
					afterClass = false;
					expectingJoin = true;
				}
				else
				{
					// handle a path expression or class name that appears
					// at the start, in the "new" HQL style or an alias that
					// appears at the start in the "old HQL stype
					IQueryable p = q.GetPersisterUsingImports( token );
					if( p != null )
					{
						// starts with the name of a mapped class (new style)
						if( joinType != JoinType.None )
						{
							throw new QueryException( "outer or full join must be followed by path expression" );
						}
						entityName = q.CreateNameFor( p.MappedClass );
						q.AddFromClass( entityName, p );
						expectingAs = true;
					}
					else if( token.IndexOf( '.' ) < 0 )
					{
						// starts with an alias (old style)
						// semi-bad thing about this: can't re-alias another alias...
						alias = token;
						expectingIn = true;
					}
					else
					{
						// starts with a path expression (new style)

						// force HQL style: from Person p inner join p.cars c
						//if (joinType==JoinType.None) throw new QueryException("path expression must be preceded by full, left, right or inner join");

						//allow ODMG OQL style: from Person p, p.cars c
						if( joinType != JoinType.None )
						{
							peParser.JoinType = joinType;
						}
						else
						{
							peParser.JoinType = JoinType.InnerJoin;
						}
						peParser.UseThetaStyleJoin = q.IsSubquery;

						ParserHelper.Parse( peParser, q.Unalias( token ), ParserHelper.PathSeparators, q );
						entityName = peParser.AddFromAssociation( q );

						joinType = JoinType.None;
						peParser.JoinType = JoinType.InnerJoin;

						if( afterFetch )
						{
							peParser.Fetch( q, entityName );
							afterFetch = false;
						}

						expectingAs = true;
					}
				}
			}
		}

		public virtual void Start( QueryTranslator q )
		{
			entityName = null;
			alias = null;
			afterIn = false;
			afterAs = false;
			afterClass = false;
			expectingJoin = false;
			expectingIn = false;
			expectingAs = false;
			joinType = JoinType.None;
		}

		public virtual void End( QueryTranslator q )
		{
			if( alias != null && expectingIn )
			{
				throw new QueryException( "in expected: <end-of-text>"
					+ " (possibly an invalid or unmapped class name was used in the query)");
			}
		}
	}
}