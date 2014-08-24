using System;
using System.Text.RegularExpressions;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	public class AnsiTrimFunction : SQLFunctionTemplate, IFunctionGrammar
	{
		public AnsiTrimFunction()
			: base(NHibernateUtil.String, "trim(?1 ?2 ?3 ?4)")
		{
		}


		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return false;
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return Regex.IsMatch(token, "LEADING|TRAILING|BOTH|FROM",
				RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		}

		#endregion
	}
}
