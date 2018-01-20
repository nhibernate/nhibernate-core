using System;
using NHibernate.SqlCommand;

namespace NHibernate.Hql
{
	public static class ParserHelper
	{
		public const string HqlVariablePrefix = ":";

		public const string HqlSeparators = " \n\r\f\t,()=<>&|+-=/*'^![]#~\\;";
		internal static readonly char[] HqlSeparatorsAsCharArray = HqlSeparators.ToCharArray();
		//NOTICE: no " or . since they are part of (compound) identifiers
		
		public const string Whitespace = " \n\r\f\t";

		public const string EntityClass = "class";

		public static bool IsWhitespace(string str)
		{
			return Whitespace.IndexOf(str, StringComparison.Ordinal) > - 1;
		}

		internal static bool HasHqlVariable(string value)
		{
			return value.IndexOf(HqlVariablePrefix, StringComparison.Ordinal) >= 0;
		}
		
		internal static bool HasHqlVariable(SqlString value)
		{
			return value.IndexOfOrdinal(HqlVariablePrefix) >= 0;
		}

		internal static bool IsHqlVariable(string value)
		{
			return value.StartsWith(HqlVariablePrefix, StringComparison.Ordinal);
		}
	}
}
