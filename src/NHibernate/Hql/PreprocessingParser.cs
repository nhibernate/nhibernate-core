using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using NHibernate.Util;

namespace NHibernate.Hql {
	/// <summary>
	/// </summary>
	public class PreprocessingParser : IParser {
		private static StringCollection operators;
		private static IDictionary collectionProps;

		static PreprocessingParser() {
			operators = new StringCollection();
			operators.Add("<=");
			operators.Add(">=");
			operators.Add("=>");
			operators.Add("=<");
			operators.Add("!=");
			operators.Add("<>");
			operators.Add("!#");
			operators.Add("!~");
			operators.Add("!<");
			operators.Add("!>");
			operators.Add("is not");
			operators.Add("not like");
			operators.Add("not in");
			operators.Add("not between");
			operators.Add("not exists");
			
			collectionProps = new Hashtable();
			collectionProps.Add("elements", "elements");
			collectionProps.Add("indices", "indices");
			collectionProps.Add("size", "size");
			collectionProps.Add("maxindex", "maxIndex");
			collectionProps.Add("minindex", "minIndex");
			collectionProps.Add("maxelement", "maxElement");
			collectionProps.Add("minelement", "minElement");
		}
		
		private IDictionary replacements;
		private bool quoted;
		private StringBuilder quotedString;
		private ClauseParser parser = new ClauseParser();
		private string lastToken;
		private string currentCollectionProp;
		
		
		public PreprocessingParser(IDictionary replacements) {
			this.replacements = replacements;
		}
		
		public void  Token(string token, QueryTranslator q) {
			
			//handle quoted strings
			if (quoted) {
				quotedString.Append(token);
			}
			if ("'".Equals(token)) {
				if (quoted) {
					token = quotedString.ToString();
				} else {
					quotedString = new StringBuilder(20).Append(token);
				}
				quoted = !quoted;
			}
			if (quoted) return;
			
			//ignore whitespace
			if (ParserHelper.IsWhitespace(token)) return;
			
			//do replacements
			string substoken = (string) replacements[token];
			token = (substoken == null) ? token : substoken;
			
			//handle HQL2 collection syntax
			if (currentCollectionProp != null) {
				if (StringHelper.OpenParen.Equals(token)) {
					return;
				} else if (StringHelper.ClosedParen.Equals(token)) {
					currentCollectionProp = null;
					return ;
				} else {
					token += StringHelper.Dot + currentCollectionProp;
				}
			}
			else {
				string prop = (string) collectionProps[token.ToLower()];
				if (prop != null) {
					currentCollectionProp = prop;
					return ;
				}
			}
			
			
			//handle <=, >=, !=, is not, not between, not in
			if (lastToken == null) {
				lastToken = token;
			} else {
				string doubleToken = (token.Length > 1)? 
					lastToken + ' ' + token : 
					lastToken + token;
				if (operators.Contains(doubleToken.ToLower())) {
					parser.Token(doubleToken, q);
					lastToken = null;
				} else {
					parser.Token(lastToken, q);
					lastToken = token;
				}
			}
			
		}
		
		public virtual void  Start(QueryTranslator q) {
			quoted = false;
			parser.Start(q);
		}
		
		public virtual void  End(QueryTranslator q) {
			if (lastToken != null) parser.Token(lastToken, q);
			parser.End(q);
			lastToken = null;
			currentCollectionProp = null;
		}
	}
}