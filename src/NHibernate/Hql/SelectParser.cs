using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using NHibernate;
using NHibernate.Util;
using NHibernate.Type;

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
		private static StringCollection countArguments = new StringCollection();
		
		static SelectParser() 
		{
			countArguments.Add("distinct");
			countArguments.Add("all");
			countArguments.Add("*");
		}

		public SelectParser()
		{
			//TODO: would be nice to use false, but issues with MS SQL
			pathExpressionParser.UseThetaStyleJoin = true;
			aggregatePathExpressionParser.UseThetaStyleJoin = true;
		}
		
		private bool ready;
		private bool aggregate;
		private bool first;
		private bool afterNew;
		private bool insideNew;
		private System.Type holderClass;

		private SelectPathExpressionParser pathExpressionParser = new SelectPathExpressionParser();
		private PathExpressionParser aggregatePathExpressionParser = new PathExpressionParser();		
		
		public void Token(string token, QueryTranslator q) 
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
			
			if (afterNew) 
			{
				afterNew=false;
				holderClass = q.GetImportedClass(token);
				if (holderClass==null) throw new QueryException("class not found: " + token);
				q.HolderClass = holderClass;
				insideNew = true;
			} 
			else if (token.Equals(StringHelper.Comma)) 
			{
				if (ready) throw new QueryException("alias or expression expected in SELECT");
				q.AppendScalarSelectToken(StringHelper.CommaSpace);
				ready = true;
			} 
			else if ( "new".Equals(lctoken) ) 
			{
				afterNew=true;
				ready=false;
			} 
			else if (StringHelper.OpenParen.Equals(token)) 
			{
				if (!aggregate && holderClass!=null && !ready) 
				{
					//opening paren in new Foo ( ... )
					ready=true;
				} 
				else if (aggregate) 
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
				if (insideNew && !aggregate && !ready) 
				{ 
					//if we are inside a new Result(), but not inside a nested function
					insideNew = false;
				}
				else if (aggregate && ready) 
				{
					q.AppendScalarSelectToken(token);
					aggregateFuncTokenList.RemoveAt(0);
					if (aggregateFuncTokenList.Count < 1) 
					{
						aggregate = false;
						ready = false;
					}

				} 
				else 
				{
					throw new QueryException("( expected before ) in select");
				}
			} 
			else if (countArguments.Contains(lctoken)) 
			{
				if (!ready || !aggregate) throw new QueryException(token + " only allowed inside aggregate function in SELECT");
				q.AppendScalarSelectToken(token);
				if ( "*".Equals(token) ) q.AddSelectScalar(NHibernate.Int32); //special case
			} 
			else if (aggregateFunctions.Contains(lctoken)) 
			{
				if (!ready) throw new QueryException(", expected before aggregate function in SELECT: " + token);
				aggregate = true;
				aggregateFuncTokenList.Insert(0, lctoken);
				ready = false;
				q.AppendScalarSelectToken(token);
				if( !AggregateHasArgs(lctoken, q) ) 
				{
					q.AddSelectScalar( AggregateType(aggregateFuncTokenList, null, q) );
					if ( !AggregateFuncNoArgsHasParenthesis(lctoken, q) ) 
					{
						aggregateFuncTokenList.RemoveAt(0);
						if (aggregateFuncTokenList.Count < 1) 
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
			else if (aggregate) 
			{
				if (!ready) throw new QueryException("( expected after aggregate function in SELECT");
				ParserHelper.Parse(aggregatePathExpressionParser, q.Unalias(token), ParserHelper.PathSeparators, q);
				
				if (aggregatePathExpressionParser.IsCollectionValued) 
				{
					q.AddCollection(
						aggregatePathExpressionParser.CollectionName, 
						aggregatePathExpressionParser.CollectionRole);
				}
				q.AppendScalarSelectToken(aggregatePathExpressionParser.WhereColumn);
				q.AddSelectScalar( AggregateType(aggregateFuncTokenList, aggregatePathExpressionParser.WhereColumnType, q) );
				aggregatePathExpressionParser.AddAssociation(q);
			} 
			else 
			{
				if (!ready) throw new QueryException(", expected in SELECT");
				
				ParserHelper.Parse(pathExpressionParser, token, ParserHelper.PathSeparators, q);
				if (pathExpressionParser.IsCollectionValued) 
				{
					q.AddCollection(
						pathExpressionParser.CollectionName, 
						pathExpressionParser.CollectionRole);
				} 
				else if (pathExpressionParser.WhereColumnType.IsEntityType) 
				{
					q.AddSelectClass(pathExpressionParser.SelectName);
				}
				q.AppendScalarSelectTokens(pathExpressionParser.WhereColumns);
				q.AddSelectScalar(pathExpressionParser.WhereColumnType);
				pathExpressionParser.AddAssociation(q);
				
				ready = false;
			}
		}
		public bool AggregateHasArgs(String funcToken, QueryTranslator q) 
		{
			IDictionary funcMap = q.AggregateFunctions;
			IQueryFunctionInfo funcInfo = (IQueryFunctionInfo)funcMap[funcToken];
			return funcInfo.IsFunctionArgs;
		}

		public bool AggregateFuncNoArgsHasParenthesis(String funcToken, QueryTranslator q) 
		{
			IDictionary funcMap = q.AggregateFunctions;
			IQueryFunctionInfo funcInfo = (IQueryFunctionInfo)funcMap[funcToken];
			return funcInfo.IsFunctionNoArgsUseParanthesis;
		}

		public IType AggregateType( ArrayList funcTokenList, IType type, QueryTranslator q) 
		{ 
			IDictionary funcMap = q.AggregateFunctions;
			IType argType = type;
			IType retType = type;
			for (int i=0; i<funcTokenList.Count; i++) 
			{
				argType = retType;
				String funcToken = (String) funcTokenList[i];
				IQueryFunctionInfo funcInfo = (IQueryFunctionInfo)funcMap[funcToken];
				retType = funcInfo.QueryFunctionType( argType, q.factory );
			}
			return retType;			
		} 

		public void Start(QueryTranslator q) 
		{
			ready = true;
			first = true;
			aggregate = false;
			afterNew = false;
			holderClass = null;
			aggregateFuncTokenList.Clear();
			aggregateFunctions = q.AggregateFunctions;
		}
		
		public void End(QueryTranslator q) 
		{
		}
	}
}