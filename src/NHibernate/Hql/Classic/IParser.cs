namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// A parser is a state machine that accepts a string of tokens,
	/// bounded by start() and end() and modifies a QueryTranslator. Parsers
	/// are NOT intended to be threadsafe. They SHOULD be reuseable
	/// for more than one token stream.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="token"></param>
		/// <param name="q"></param>
		void Token( string token, QueryTranslator q );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		void Start( QueryTranslator q );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="q"></param>
		void End( QueryTranslator q );
	}
}