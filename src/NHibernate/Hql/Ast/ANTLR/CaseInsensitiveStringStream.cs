using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	/// <summary>
	/// Look ahead for tokenizing is all lowercase, whereas the original case of an input stream is preserved.
	/// Copied from http://www.antlr.org/wiki/pages/viewpage.action?pageId=1782
	///</summary>
	public class CaseInsensitiveStringStream : ANTLRStringStream
	{
		public CaseInsensitiveStringStream(char[] data, int numberOfActualCharsInArray) : base(data, numberOfActualCharsInArray) { }

		public CaseInsensitiveStringStream() { }

		public CaseInsensitiveStringStream(string input) : base(input) { }

		// Only the lookahead is converted to lowercase. The original case is preserved in the stream.
		public override int LA(int i)
		{
			if (i == 0)
			{
				return 0;
			}

			if (i < 0)
			{
				i++;
			}

			if (((p + i) - 1) >= n)
			{
				return (int)CharStreamConstants.EOF;
			}

			return Char.ToLowerInvariant(data[(p + i) - 1]); // This is how "case insensitive" is defined, i.e., could also use a special culture...
		}
	}
}
