namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// Parses the having clause of a hibernate query and translates it to an
	/// SQL having clause.
	/// </summary>
	public class HavingParser : WhereParser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		/// <param name="token"></param>
		protected override void AppendToken( QueryTranslator q, string token )
		{
			// a String.Empty can get passed in here.  If that occurs
			// then don't create a new SqlString for it - just ignore
			// it since it adds nothing to the sql being generated.
			if( token != null && token.Length > 0 )
			{
				q.AppendHavingToken( token );
			}
		}
	}
}