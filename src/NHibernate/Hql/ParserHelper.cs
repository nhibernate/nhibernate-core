using System;

using NHibernate.Util;

namespace NHibernate.Hql {

	public class ParserHelper {

		public static readonly string HqlVariablePrefix = ":";
	
		public static readonly string HqlSeparators = " \n\r\f\t,()=<>&|+-=/*'^![]#~";
		//NOTICE: no " or . since they are part of (compound) identifiers
		public static readonly string PathSeparators = ".";
	
		public static readonly string Whitespace = " \n\r\f\t";
	
		public static bool isWhitespace(string str) {
			return Whitespace.IndexOf(str) > -1;
		}
	
		private ParserHelper() {
			//cannot instantiate
		}
	
		public static void Parse(IParser p, string text, string separators, QueryTranslator q) {
			StringTokenizer tokens = new StringTokenizer(text, separators, true);
			p.Start(q);
			foreach(string tok in tokens)
				p.Token(tok, q);
			p.End(q);
		}
	}
}