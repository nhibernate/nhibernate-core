//$Id$
using System;
using System.Collections;
using NHibernate;
using NHibernate.Util;

namespace NHibernate.Hql
{
	
	/// <summary> 
	/// Parses the from clause of a hibernate query, looking for tables and
	/// aliases for the SQL query.
	/// </summary>
	
	public class FromParser : IParser
	{
		private bool   expectingIn;
		private bool   expectingComma;
		private bool   fromClass;
		private string name;
		private PathExpressionParser peParser;

		public FromParser()
		{
			peParser = new PathExpressionParser();
		}
		
		
		public void Token(string token, QueryTranslator q)
		{
			
			string lcToken = token.ToLower();
			if (lcToken.Equals("class"))
			{
				fromClass = true;
			}
			else if (expectingIn)
			{
				if (lcToken.Equals("in"))
				{
					expectingIn = false;
				}
				else
				{
					throw new QueryException("IN expected but found: " + token);
				}
			}
			else if (expectingComma)
			{
				if (token.Equals(StringHelper.Comma))
				{
					expectingComma = false;
					fromClass = false;
				}
				else
				{
					throw new QueryException(", or WHERE expected but found: " + token);
				}
			}
			else
			{
				if (name == null)
				{
					name = token;
					expectingIn = true;
				}
				else
				{
					if (fromClass)
					{
						q.AddFromType(name, token);
					}
					else
					{
						ParserHelper.Parse(peParser, token, ParserHelper.PathSeparators, q);
						peParser.AddFromCollection(q, name);
					}
					name = null;
					expectingComma = true;
				}
			}
		}
		
		public virtual void  Start(QueryTranslator q)
		{
			expectingIn = false;
			expectingComma = false;
			name = null;
			fromClass = false;
		}
		
		public virtual void  End(QueryTranslator q)
		{
		}
	}
}