//$Id$
using System;
using System.Collections;
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

		private PathExpressionParser pathExpressionParser;
		
		public OrderByParser()
		{
			pathExpressionParser = new PathExpressionParser();
		}
		
		public void Token(string token, QueryTranslator q)
		{
			
			if (q.IsName(StringHelper.Root(token)))
			{
				ParserHelper.Parse(pathExpressionParser, token, ParserHelper.PathSeparators, q);
				q.AppendOrderByToken(pathExpressionParser.WhereColumn);
				q.AddJoin(pathExpressionParser.WhereJoin);
			}
			else
			{
				q.AppendOrderByToken(token);
			}
		}
		
		public void Start(QueryTranslator q)
		{
		}
		
		public void End(QueryTranslator q)
		{
		}
	}
}