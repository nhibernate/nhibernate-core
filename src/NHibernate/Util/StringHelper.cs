using System;
using System.Text;
using System.Collections;

namespace NHibernate.Util {
	
	public sealed class StringHelper {
		
		public const char Dot = '.';
		public const char Underscore = '_';
		public const string CommaSpace = ", ";
		public const string Comma = ",";
		public const string OpenParen = "(";
		public const string ClosedParen = ")";
		public const string SqlParameter = "?";

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
			// sometimes a null value will get passed in here -> SqlWhereStrings are a good example
			if(template==null) return null;

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
		public static string[] Split(string separators, string list) 
		{
			return list.Split(separators.ToCharArray());
		}

		/// <summary>
		/// Splits the String using the StringTokenizer.  
		/// </summary>
		/// <param name="separators">separators for the tokens of the list</param>
		/// <param name="list">the string that will be broken into tokens</param>
		/// <param name="include">true to include the seperators in the tokens.</param>
		/// <returns></returns>
		/// <remarks>
		/// This is more powerful than Split because you have the option of including or 
		/// not including the seperators in the tokens.
		/// </remarks>
		public static string[] Split(string separators, string list, bool include) 
		{
			StringTokenizer tokens = new StringTokenizer(list, separators, include);
			ArrayList results = new ArrayList();
			foreach(string token in tokens) 
			{
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

		/// <summary>
		/// Takes a fully qualified type name and returns the full name of the 
		/// Class - includes namespaces.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static string GetFullClassname(string typeName) 
		{
			return typeName.Trim().Split(' ', ',')[0];
		}

		/// <summary>
		/// Takes a fully qualifed type name (can include the assembly) and just returns
		/// the name of the Class.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static string GetClassname(string typeName) 
		{
			string[] splitClassname = GetFullClassname(typeName).Split('.');

			return splitClassname[splitClassname.Length-1];

		}

		public static string Qualifier(string qualifiedName) {
			int loc = qualifiedName.LastIndexOf(".");
			if ( loc< 0 ) {
				return String.Empty;
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
			return (suffix == null) ?
				name :
				name + suffix;
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
			
			// if there is no value in the array then return no string...
			if(len==0) return String.Empty;

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
	}
}
