//$Id$
using System;
using System.Collections;
using NHibernate;

namespace NHibernate.Hql
{
	
	/// <summary> 
	/// Parses the hibernate query into its constituent clauses.
	/// </summary>
	public class ClauseParser : IParser
	{
		
		private IParser child;
		private IList   selectTokens;
		private bool    cacheSelectTokens = false;
		private bool    byExpected = false;
		private bool    enableSubselect = false;
		
		public virtual void Token(string token, QueryTranslator q)
		{
			string lcToken = token.ToLower();
			
			if (byExpected && !lcToken.Equals("by"))
				throw new QueryException("BY expected after GROUP or ORDER: " + token);
			
			
			if (!enableSubselect && lcToken.Equals("select"))
			{
				selectTokens      = new ArrayList();
				cacheSelectTokens = true;
			}
			else if (!enableSubselect && lcToken.Equals("from"))
			{
				child = new FromParser();
				child.Start(q);
				cacheSelectTokens = false;
			}
			else if (!enableSubselect && lcToken.Equals("where"))
			{
				enableSubselect = true;
				EndChild(q);
				child = new WhereParser();
				child.Start(q);
			}
			else if (lcToken.Equals("order"))
			{
				EndChild(q);
				child      = new OrderByParser();
				byExpected = true;
			}
			else if (lcToken.Equals("having"))
			{
				EndChild(q);
				enableSubselect = true;
				child           = new HavingParser();
				child.Start(q);
			}
			else if (lcToken.Equals("group"))
			{
				EndChild(q);
				child = new GroupByParser();
				byExpected = true;
			}
			else if (lcToken.Equals("by"))
			{
				if (!byExpected)
					throw new QueryException("GROUP or ORDER expected before BY");
				child.Start(q);
				byExpected = false;
			}
			else
			{
				if (cacheSelectTokens)
				{
					selectTokens.Add(token);
				}
				else
				{
					if (child == null)
					{
						throw new QueryException("query must begin with SELECT or FROM: " + token);
					}
					else
					{
						child.Token(token, q);
					}
				}
			}
		}
		
		private void  EndChild(QueryTranslator q)
		{
			if (child == null)
			{
				//null child could occur for no from clause in a filter
				cacheSelectTokens = false;
			}
			else
			{
				child.End(q);
			}
		}
		
		public virtual void  Start(QueryTranslator q)
		{
		}
		
		public virtual void  End(QueryTranslator q)
		{
			EndChild(q);
			if (selectTokens != null)
			{
				child = new SelectParser();
				child.Start(q);
				foreach (string item in selectTokens)
				{
					Token(item, q);
				}
				child.End(q);
			}
			byExpected        = false;
			enableSubselect   = false;
			cacheSelectTokens = false;
		}
	}
}