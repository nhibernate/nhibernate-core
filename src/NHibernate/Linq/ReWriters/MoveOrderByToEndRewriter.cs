using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.ReWriters
{
	public class MoveOrderByToEndRewriter
	{
		public static void ReWrite(QueryModel queryModel)
		{
			int len = queryModel.BodyClauses.Count;
			for(int i=0; i<len; i++)
			{
				if (queryModel.BodyClauses[i] is OrderByClause)
				{
					// If we find an order by clause, move it to the end of the list.
					// This preserves the ordering of multiple orderby clauses if there are any.
					IBodyClause clause = queryModel.BodyClauses[i];
					queryModel.BodyClauses.RemoveAt(i);
					queryModel.BodyClauses.Add(clause);
					i--;
					len--;
				}
			}
		}
	}
}
