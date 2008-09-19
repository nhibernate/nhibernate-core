using System;

namespace NHibernate.Linq.Util
{
	public static class Guard
	{
		public static void AgainstNull(object obj, string parameterName)
		{
			if (obj == null)
				throw new ArgumentNullException(parameterName);
		}
	}
}