using System;
using NHibernate.Util;

namespace NHibernate.Dialect.Function
{
	public class CommonGrammar: IFunctionGrammar
	{
		#region IFunctionGrammar Members

		public bool IsSeparator(string token)
		{
			return StringHelper.CommaSpace.Equals(token) || StringHelper.Comma.Equals(token);
		}

		public bool IsKnownArgument(string token)
		{
			return false;
		}

		#endregion
	}
}
