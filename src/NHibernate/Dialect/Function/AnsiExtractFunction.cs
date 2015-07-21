using System;
using System.Text.RegularExpressions;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// <![CDATA[
	/// <extract expression> ::=
	///					EXTRACT <left paren> <extract field> FROM <extract source> <right paren>
	/// 
	///<extract field > ::=
	///			<datetime field> | <time zone field >
	/// 
	/// <datetime field> ::= 
	///			YEAR |
	///			MONTH |
	///			DAY |
	///			HOUR |
	///			MINUTE |
	///			SECOND
	/// 
	///<time zone field> ::= 
	///			TIMEZONE_HOUR	| 
	///			TIMEZONE_MINUTE
	/// ]]>
	/// </remarks>
	[Serializable]
	public class AnsiExtractFunction: SQLFunctionTemplate, IFunctionGrammar
	{
		public AnsiExtractFunction()
			: base(NHibernateUtil.Int32, "extract(?1 ?2 ?3)")
		{
		}

		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return false;
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return Regex.IsMatch(token, "YEAR|MONTH|DAY|HOUR|MINUTE|SECOND|TIMEZONE_HOUR|TIMEZONE_MINUTE|FROM",
				RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}

		#endregion
	}
}
