using System;

namespace NHibernate.Hql {
	/// <summary> 
	/// Parses the having clause of a hibernate query and translates it to an
	/// SQL having clause.
	/// </summary>
	public class HavingParser : WhereParser {
		
		protected override void AppendToken(QueryTranslator q, string token) {
			q.AppendHavingToken(token);
		}
	}
}