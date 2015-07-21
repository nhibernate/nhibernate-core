using System;

namespace NHibernate.SqlCommand.Parser
{
	/// <summary>
	/// <see cref="SqlToken"/> token types.
	/// </summary>
	[Flags]
	public enum SqlTokenType
	{
		/// <summary>
		/// Whitespace
		/// </summary>
		Whitespace = 0x1,

		/// <summary>
		/// Single line comment (preceeded by --) or multi-line comment (terminated by /* and */)
		/// </summary>
		Comment = 0x2,

		/// <summary>
		/// Keywords, operators or undelimited identifiers. 
		/// </summary>
		Text = 0x4,

		/// <summary>
		/// Delimited identifiers or string literals.
		/// </summary>
		DelimitedText = 0x8,

		/// <summary>
		/// A query parameter.
		/// </summary>
		Parameter = 0x10,

		/// <summary>
		/// List separator, the ',' character.
		/// </summary>
		Comma = 0x20,

		/// <summary>
		/// Begin of an expression block, consisting of a '(' character.
		/// </summary>
		BracketOpen = 0x40,

		/// <summary>
		/// End of an expression block, consisting of a ')' character.
		/// </summary>
		BracketClose = 0x80,

		/// <summary>
		/// Tokens for begin or end of expression blocks.
		/// </summary>
		AllBrackets = BracketOpen | BracketClose,

		/// <summary>
		/// Includes all token types except whitespace or comments
		/// </summary>
		AllExceptWhitespaceOrComment = AllExceptWhitespace & ~Comment, 

		/// <summary>
		/// Includes all token types except whitespace
		/// </summary>
		AllExceptWhitespace = All & ~Whitespace,
		
		/// <summary>
		/// Includes all token types
		/// </summary>
		All = Whitespace | Comment | Text | Comma | BracketOpen | BracketClose | Parameter | DelimitedText,
	}
}
