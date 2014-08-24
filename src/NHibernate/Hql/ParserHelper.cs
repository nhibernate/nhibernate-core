namespace NHibernate.Hql
{
	public static class ParserHelper
	{
		public const string HqlVariablePrefix = ":";

		public const string HqlSeparators = " \n\r\f\t,()=<>&|+-=/*'^![]#~\\;";
		//NOTICE: no " or . since they are part of (compound) identifiers
		
		public const string Whitespace = " \n\r\f\t";

		public const string EntityClass = "class";

		public static bool IsWhitespace(string str)
		{
			return Whitespace.IndexOf(str) > - 1;
		}
	}
}