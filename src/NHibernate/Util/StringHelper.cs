using System;
using System.Text;
using System.Collections;

namespace NHibernate.Util {
	
	public sealed class StringHelper {
		
		public const string EmptyString = "";
		public const char Dot = '.';
		public const char Underscore = '_';
		public const string CommaSpace = ", ";
		public const string Comma = ",";
		public const string OpenParen = "(";
		public const string ClosedParen = ")";

		[Obsolete("Use String.Join() instead of this method. It does the same thing")]
		public static string Join(string separator, string[] strings) {
			return string.Join(separator, strings);
		}

		public static string Repeat(string str, int times) {
			StringBuilder buf = new StringBuilder(str.Length * times);
			for (int i=0; i<times; i++)
				buf.Append(str);
			return buf.ToString();
		}

		public static string Replace(string template, string placeholder, string replacement) {
			int loc = template.IndexOf(placeholder);
			if (loc<0) {
				return template;
			} else {
				return new StringBuilder( template.Substring(0, loc) )
					.Append(replacement)
					.Append( Replace(
					template.Substring( loc + placeholder.Length ),
					placeholder,
					replacement
					) ).ToString();
			}
		}

		public static string ReplaceOnce(string template, string placeholder, string replacement) {
			int loc = template.IndexOf(placeholder);
			if (loc < 0) {
				return template;
			} else {
				return new StringBuilder( template.Substring(0, loc) )
					.Append(replacement)
					.Append( template.Substring( loc + placeholder.Length ) )
					.ToString();
			}
		}

		/// <summary>
		/// Just a façade for calling string.Split()
		/// We don't use our StringTokenizer because string.Split() is
		/// more efficient (but it only works when we don't want to retrieve the delimiters)
		/// </summary>
		/// <param name="separators">separators for the tokens of the list</param>
		/// <param name="list">the string that will be broken into tokens</param>
		/// <returns></returns>
		public static string[] Split(string separators, string list) {
			return list.Split(separators.ToCharArray());
		}

		public static string[] Split(string separators, string list, bool include) {
			StringTokenizer tokens = new StringTokenizer(list, separators, include);
			ArrayList results = new ArrayList();
			foreach(string token in tokens) {
				results.Add( token );
			}
			return (string[]) results.ToArray(typeof(string));
		}

		public static string Unqualify(string qualifiedName) {
			return Unqualify(qualifiedName, ".");
		}

		public static string Unqualify(string qualifiedName, string seperator) {
			return qualifiedName.Substring( qualifiedName.LastIndexOf(seperator) + 1 );
		}

		public static string Qualifier(string qualifiedName) {
			int loc = qualifiedName.LastIndexOf(".");
			if ( loc< 0 ) {
				return EmptyString;
			} else {
				return qualifiedName.Substring(0, loc);
			}
		}

		public static string[] Suffix(string[] columns, string suffix) {
			if (suffix == null)
				return columns;
			string[] qualified = new string[columns.Length];
			for ( int i=0; i<columns.Length; i++) {
				qualified[i] = Suffix(columns[i], suffix);
			}
			return qualified;
		}

		public static string Suffix(string name, string suffix) {
			if (suffix == null)
				return name;

			char quote = name[0];
			bool nameEscaped = Dialect.Dialect.Quote.IndexOf(quote) > -1;
			StringBuilder nameBuffer = new StringBuilder(30);

			if (nameEscaped) {
				nameBuffer.Append( name.Substring(1, name.Length-1) ).Append(suffix);
			} else {
				nameBuffer.Append(name).Append(suffix);
			}

			if (nameEscaped) {
				nameBuffer.Insert(0, quote);
				nameBuffer.Append(quote);
			}
			return nameBuffer.ToString();
		}

		public static string[] Prefix(string [] columns, string prefix) {
			if (prefix == null)
				return columns;
			string[] qualified = new string[columns.Length];
			for (int i=0; i<columns.Length; i++) {
				qualified[i] = prefix + columns[i];
			}
			return qualified;
		}

		public static string Root(string qualifiedName) {
			int loc = qualifiedName.IndexOf(".");
			return (loc<0)
				? qualifiedName
				: qualifiedName.Substring(0, loc);
		}

		public static bool BooleanValue(string tfString) {
			string trimmed = tfString.Trim().ToLower();
			return trimmed.Equals("true") || trimmed.Equals("t");
		}

		public static string ToString(object[] array) {
			int len = array.Length;
			StringBuilder buf = new StringBuilder(len * 12);
			for (int i=0; i<len - 1; i++) {
				buf.Append( array[i] ).Append(StringHelper.CommaSpace);
			}
			return buf.Append( array[len-1]).ToString();
		}

		public static string[] Multiply(string str, IEnumerator placeholders, IEnumerator replacements) {
			string[] result = new string[] { str };
			while( placeholders.MoveNext() ) {
				replacements.MoveNext();
				result = Multiply( result, placeholders.Current as string, replacements.Current as string[]);
			}
			return result;
		}

		public static string[] Multiply(string[] strings, string placeholder, string[] replacements) {
			string[] results = new string[replacements.Length * strings.Length ];
			int n=0;
			for ( int i=0; i<replacements.Length; i++ ) {
				for (int j=0; j<strings.Length; j++) {
					results[n++] = ReplaceOnce(strings[j], placeholder, replacements[i]);
				}
			}
			return results;
		}

		public static string UnQuote(string name) {
			return ( Dialect.Dialect.Quote.IndexOf( name[0] ) > -1 )
				? name.Substring(1, name.Length-1)
				: name;
		}

		public static void UnQuoteInPlace(string[] names) {
			for (int i=0; i<names.Length; i++)
				names[i] = UnQuote(names[i]);
		}

		public static string[] UnQuote(string[] names) {
			string[] unquoted = new string[ names.Length ];
			for (int i=0; i<names.Length; i++)
				unquoted[i] = UnQuote(names[i]);
			return unquoted;
		}
	}
}
