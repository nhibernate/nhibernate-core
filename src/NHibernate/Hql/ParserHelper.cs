using NHibernate.Util;

namespace NHibernate.Hql
{
	/// <summary></summary>
	public class ParserHelper
	{
		/// <summary></summary>
		public const string HqlVariablePrefix = ":";
		/// <summary></summary>
		public const string HqlSeparators = " \n\r\f\t,()=<>&|+-=/*'^![]#~\\";
		//NOTICE: no " or . since they are part of (compound) identifiers
		/// <summary></summary>
		public const string PathSeparators = ".";
		/// <summary></summary>
		public const string Whitespace = " \n\r\f\t";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsWhitespace( string str )
		{
			return Whitespace.IndexOf( str ) > - 1;
		}

		private ParserHelper()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="text"></param>
		/// <param name="seperators"></param>
		/// <param name="q"></param>
		public static void Parse( IParser p, string text, string seperators, QueryTranslator q )
		{
			StringTokenizer tokens = new StringTokenizer( text, seperators, true );
			p.Start( q );
			foreach( string token in tokens )
			{
				p.Token( token, q );
			}
			p.End( q );
		}
	}
}