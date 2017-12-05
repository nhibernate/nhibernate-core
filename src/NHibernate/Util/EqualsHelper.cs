using System;

namespace NHibernate.Util
{
	public static class EqualsHelper
	{
		[Obsolete("Please use object.Equals(object, object) instead.")]
		public new static bool Equals(object x, object y)
		{
			return object.Equals(x, y);
		}
	}
}
