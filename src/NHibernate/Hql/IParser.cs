//$Id$
using System;

namespace NHibernate.Hql {
	/// <summary> 
	/// A parser is a state machine that accepts a string of tokens,
	/// bounded by start() and end() and modifies a QueryTranslator. Parsers
	/// are NOT intended to be threadsafe. They SHOULD be reuseable
	/// for more than one token stream.
	/// </summary>
	public interface IParser {
		void Token(string token, QueryTranslator q);
		void Start(QueryTranslator q);
		void End(QueryTranslator q);
	}
}