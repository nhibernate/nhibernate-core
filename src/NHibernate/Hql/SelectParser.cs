//$Id$
using System;
using System.Collections;
using System.Collections.Specialized;
using NHibernate;
using NHibernate.Util;

namespace NHibernate.Hql
{
	
	/// <summary>
	/// Parsers the select clause of a hibernate query, looking
	/// for a table (well, really class) alias.
	/// </summary>
	public class SelectParser : IParser
	{
		private static StringCollection aggregateFunctions = new StringCollection();
		private static StringCollection countArguments = new StringCollection();
		
		private bool ready;
		private bool aggregate;
		private bool count;
		private bool first;
		private SelectPathExpressionParser pathExpressionParser;
		private PathExpressionParser       aggregatePathExpressionParser;

		static SelectParser()
		{
			aggregateFunctions.Add("count");
			aggregateFunctions.Add("avg");
			aggregateFunctions.Add("max");
			aggregateFunctions.Add("min");
			aggregateFunctions.Add("sum");
				
			countArguments.Add("distinct");
			countArguments.Add("all");
			countArguments.Add("*");
		}

		public SelectParser()
		{
			pathExpressionParser = new SelectPathExpressionParser();
			aggregatePathExpressionParser = new PathExpressionParser();
		}
		
		
		public void  Token(string token, QueryTranslator q)
		{
			
			string lctoken = token.ToLower();
			
			if (first)
			{
				first = false;
				if (lctoken.Equals("distinct"))
				{
					q.Distinct = true;
					return ;
				}
				else if (lctoken.Equals("all"))
				{
					q.Distinct = false;
					return ;
				}
			}
			
			if (token.Equals(StringHelper.Comma))
			{
				if (ready)
					throw new QueryException("alias or expression expected in SELECT");
				q.AppendScalarSelectToken(StringHelper.CommaSpace);
				ready = true;
			}
			else if (StringHelper.OpenParen.Equals(token))
			{
				if (aggregate)
				{
					q.AppendScalarSelectToken(token);
				}
				else
				{
					throw new QueryException("aggregate function expected before ( in SELECT");
				}
				ready = true;
			}
			else if (StringHelper.ClosedParen.Equals(token))
			{
				if (aggregate && ready)
				{
					q.AppendScalarSelectToken(token);
				}
				else
				{
					throw new QueryException("( expected before ) in select");
				}
				count = false;
				aggregate = false;
				ready = false;
			}
			else if (countArguments.Contains(lctoken))
			{
				if (!ready || !aggregate)
					throw new QueryException(token + " only allowed inside aggregate function in SELECT");
				q.AppendScalarSelectToken(token);
			}
			else if (aggregateFunctions.Contains(lctoken))
			{
				if (!ready)
					throw new QueryException(", expected before aggregate function in SELECT: " + token);
				/*if ( !q.supportsScalars() ) throw new QueryException(
				"aggregate functions may only be used in the select clause of iterate() queries: " + token
				);*/
				if (lctoken.Equals("count"))
				{
					q.AddScalarType(NHibernate.Integer);
					count = true;
				}
				aggregate = true;
				ready = false;
				q.AppendScalarSelectToken(token);
			}
			else if (aggregate)
			{
				if (!ready)
					throw new QueryException("( expected after aggregate function in SELECT");
				ParserHelper.Parse(aggregatePathExpressionParser, token, ParserHelper.PathSeparators, q);
				
				if (aggregatePathExpressionParser.IsCollectionValued())
				{
					q.AddCollection(aggregatePathExpressionParser.CollectionName, aggregatePathExpressionParser.CollectionRole);
				}
				q.AppendScalarSelectToken(aggregatePathExpressionParser.WhereColumn);
				if (!count)
				{
					q.AddScalarType(aggregatePathExpressionParser.WhereColumnType);
				}
				q.AddJoin(aggregatePathExpressionParser.WhereJoin);
			}
			else
			{
				if (!ready)
					throw new QueryException(", expected in SELECT");
				
				ParserHelper.Parse(pathExpressionParser, token, ParserHelper.PathSeparators, q);
				if (pathExpressionParser.IsCollectionValued())
				{
					q.AddCollection(pathExpressionParser.CollectionName, pathExpressionParser.CollectionRole);
				}
				else if (pathExpressionParser.WhereColumnType.IsEntityType)
				{
					q.AddReturnType(pathExpressionParser.SelectName);
				}
				q.AppendScalarSelectTokens(pathExpressionParser.WhereColumns);
				q.AddScalarType(pathExpressionParser.WhereColumnType);
				q.AddJoin(pathExpressionParser.WhereJoin);
				
				ready = false;
			}
		}
		
		public void Start(QueryTranslator q)
		{
			ready = true;
			first = true;
			aggregate = false;
			count = false;
		}
		
		public void End(QueryTranslator q)
		{
		}
	}
}