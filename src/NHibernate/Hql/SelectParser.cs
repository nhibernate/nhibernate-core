using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Dialect;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Dialect.Function;

namespace NHibernate.Hql
{
	/// <summary>
	/// Parsers the select clause of a hibernate query, looking
	/// for a table (well, really class) alias.
	/// </summary>
	public class SelectParser : IParser
	{
		private ArrayList aggregateFuncTokenList = new ArrayList();
		private static IDictionary aggregateFunctions = new Hashtable();
		private static ISet countArguments = new HashedSet();

		/// <summary></summary>
		static SelectParser()
		{
			countArguments.Add( "distinct" );
			countArguments.Add( "all" );
			countArguments.Add( "*" );
		}

		/// <summary></summary>
		public SelectParser()
		{
			//TODO: would be nice to use false, but issues with MS SQL
			//TODO: H2.0.3 why not getting info from Dialect?
			pathExpressionParser.UseThetaStyleJoin = true;
			aggregatePathExpressionParser.UseThetaStyleJoin = true;
		}

		private bool ready;
		private bool aggregate;
		private bool first;
		private bool afterNew;
		private bool insideNew;
		private bool aggregateAddSelectScalar;
		private bool afterAggregatePath;
		private System.Type holderClass;

		private SelectPathExpressionParser pathExpressionParser = new SelectPathExpressionParser();
		private PathExpressionParser aggregatePathExpressionParser = new PathExpressionParser();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		public void Token( string token, QueryTranslator q )
		{
			string lctoken = token.ToLower( System.Globalization.CultureInfo.InvariantCulture );

			if( first )
			{
				first = false;
				if( lctoken.Equals( "distinct" ) )
				{
					q.Distinct = true;
					return;
				}
				else if( lctoken.Equals( "all" ) )
				{
					q.Distinct = false;
					return;
				}
			}

			if( afterNew )
			{
				afterNew = false;
				holderClass = q.GetImportedClass( token );
				if( holderClass == null )
				{
					throw new QueryException( "class not found: " + token );
				}
				q.HolderClass = holderClass;
				insideNew = true;
			}
			else if( token.Equals( StringHelper.Comma ) )
			{
				if( ready )
				{
					throw new QueryException( "alias or expression expected in SELECT" );
				}
				q.AppendScalarSelectToken( StringHelper.CommaSpace );
				ready = true;
			}
			else if( "new".Equals( lctoken ) )
			{
				afterNew = true;
				ready = false;
			}
			else if( StringHelper.OpenParen.Equals( token ) )
			{
				if( !aggregate && holderClass != null && !ready )
				{
					//opening paren in new Foo ( ... )
					ready = true;
				}
				else if( aggregate )
				{
					q.AppendScalarSelectToken( token );
				}
				else
				{
					throw new QueryException( "aggregate function expected before ( in SELECT" );
				}
				ready = true;
			}
			else if( StringHelper.ClosedParen.Equals( token ) )
			{
				if( insideNew && !aggregate && !ready )
				{
					//if we are inside a new Result(), but not inside a nested function
					insideNew = false;
				}
				else if( aggregate && ready )
				{
					q.AppendScalarSelectToken( token );
					aggregateFuncTokenList.RemoveAt( 0 );
					if( aggregateFuncTokenList.Count < 1 )
					{
						aggregate = false;
						ready = false;
					}

				}
				else
				{
					throw new QueryException( "( expected before ) in select" );
				}
			}
			else if( countArguments.Contains( lctoken ) )
			{
				if( !ready || !aggregate )
				{
					throw new QueryException( token + " only allowed inside aggregate function in SELECT" );
				}
				q.AppendScalarSelectToken( token );
				if( "*".Equals( token ) && !afterAggregatePath)
				{
					q.AddSelectScalar( NHibernateUtil.Int32 );
				} //special case
				afterAggregatePath = false;
			}
			else if ( GetFunction( lctoken, q ) != null && token == q.Unalias( token ) )
			{
				// the name of an SQL function
				if( !ready )
				{
					throw new QueryException( ", expected before aggregate function in SELECT: " + token );
				}
				aggregate = true;
				afterAggregatePath = false;
				aggregateAddSelectScalar = true;
				aggregateFuncTokenList.Insert( 0, lctoken );
				ready = false;
				q.AppendScalarSelectToken( token );
				if( !AggregateHasArgs( lctoken, q ) )
				{
					q.AddSelectScalar( AggregateType( aggregateFuncTokenList, null, q ) );
					if( !AggregateFuncNoArgsHasParenthesis( lctoken, q ) )
					{
						aggregateFuncTokenList.RemoveAt( 0 );
						if( aggregateFuncTokenList.Count < 1 )
						{
							aggregate = false;
							ready = false;
						}
						else
						{
							ready = true;
						}
					}
				}
			}
			else if( aggregate )
			{
				bool constantToken = false;
				if( !ready )
				{
					throw new QueryException( "( expected after aggregate function in SELECT" );
				}
				try
				{
					ParserHelper.Parse( aggregatePathExpressionParser, q.Unalias( token ), ParserHelper.PathSeparators, q );
				}
				catch (QueryException)
				{
					constantToken = true;
				}
				afterAggregatePath = true;

				if( constantToken )
				{
					q.AppendScalarSelectToken( token );
				}
				else
				{
					if( aggregatePathExpressionParser.IsCollectionValued )
					{
						q.AddCollection(
							aggregatePathExpressionParser.CollectionName,
							aggregatePathExpressionParser.CollectionRole );
					}
					q.AppendScalarSelectToken( aggregatePathExpressionParser.WhereColumn );
					if( aggregateAddSelectScalar )
					{
						q.AddSelectScalar( AggregateType( aggregateFuncTokenList, aggregatePathExpressionParser.WhereColumnType, q ) );
						aggregateAddSelectScalar = false;
					}
					aggregatePathExpressionParser.AddAssociation( q );
				}
			}
			else
			{
				if( !ready )
				{
					throw new QueryException( ", expected in SELECT" );
				}

				ParserHelper.Parse( pathExpressionParser, q.Unalias( token ), ParserHelper.PathSeparators, q );
				if( pathExpressionParser.IsCollectionValued )
				{
					q.AddCollection(
						pathExpressionParser.CollectionName,
						pathExpressionParser.CollectionRole );
				}
				else if( pathExpressionParser.WhereColumnType.IsEntityType )
				{
					q.AddSelectClass( pathExpressionParser.SelectName );
				}
				q.AppendScalarSelectTokens( pathExpressionParser.WhereColumns );
				q.AddSelectScalar( pathExpressionParser.WhereColumnType );
				pathExpressionParser.AddAssociation( q );

				ready = false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="funcToken"></param>
		/// <param name="q"></param>
		/// <returns></returns>
		public bool AggregateHasArgs( String funcToken, QueryTranslator q )
		{
			return GetFunction( funcToken, q ).HasArguments;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="funcToken"></param>
		/// <param name="q"></param>
		/// <returns></returns>
		public bool AggregateFuncNoArgsHasParenthesis( string funcToken, QueryTranslator q )
		{
			return GetFunction( funcToken, q ).HasParenthesesIfNoArguments;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="funcTokenList"></param>
		/// <param name="type"></param>
		/// <param name="q"></param>
		/// <returns></returns>
		public IType AggregateType( ArrayList funcTokenList, IType type, QueryTranslator q )
		{
			IType argType = type;
			IType retType = type;
			for( int i = 0; i < funcTokenList.Count; i++ )
			{
				argType = retType;
				string funcToken = ( string ) funcTokenList[ i ];
				retType = GetFunction( funcToken, q ).ReturnType( argType, q.Factory ) ;
			}
			return retType;
		}

		private ISQLFunction GetFunction( string name, QueryTranslator q ) 
		{
			return (ISQLFunction) q.Factory.Dialect.Functions[ name ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void Start( QueryTranslator q )
		{
			ready = true;
			first = true;
			aggregate = false;
			afterNew = false;
			holderClass = null;
			aggregateFuncTokenList.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		public void End( QueryTranslator q )
		{
		}
	}
}
