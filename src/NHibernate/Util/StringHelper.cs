using System;
using System.Text;

namespace NHibernate.Util {
	
	public sealed class StringHelper {
		
		public const string EmptyString = "";
		public const char Dot = '.';
		public const char Underscore = '_';
		public const string CommaSpace = ", ";
		public const string Comma = ",";
		public const string OpenParen = "(";
		public const string ClosedParen = ")";

		public static string Join(string seperator, string[] strings) {
			int length = strings.Length;
			if (length == 0)
				return EmptyString;
			StringBuilder buf = new StringBuilder( length * strings[0].Length )
				.Append(strings[0]);
			for (int i=1; i<length; i++) {
				buf.Append(seperator).Append(strings[i]);
			}
			return buf.ToString();
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
					.Append( template.Substring( loc + placeholder.Length() ) )
					.ToString();
			}
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
			if (suffix==null)
				return columns;
			string[] qualified = new string[columns.Length];
			for (int i=0; i<columns.Length; i++) {
				qualified[i] = Suffix(columns[i], suffix);
			}
			return qualified;
		}
		
		public static string Suffix(string name, string suffix) {
			if (suffix==null)
				return name;

			char quote = name[0];
			bool nameEscaped = Dialect.

	}
}
