using System;
using NHibernate;
using NHibernate.Portable;

namespace NHibernate.Hql {
	public class ParserHelper {
		
		public const string HqlVariablePrefix = ":";
		public const string HqlSeparators     = " \n\r\f\t,()=<>&|+-=/*'^![]#~";
		//NOTICE: no " or . since they are part of (compound) identifiers
		public const string PathSeparators    = ".";
		public const string Whitespace        = " \n\r\f\t";
		
		public static bool IsWhitespace(string str) {
			return Whitespace.IndexOf(str) > - 1;
		}
		
		private ParserHelper() {
		}
		
		public static void Parse(IParser p, string text, string seperators, QueryTranslator q) {
			StringTokenizer tokens = new StringTokenizer(text, seperators, true);
			p.Start(q);
			while (tokens.HasMoreTokens()) {
				p.Token(tokens.NextToken(), q);
			}
			p.End(q);
		}
	}
}