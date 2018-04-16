using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// A HQL only cast for helping HQL knowing the type. Does not generates any actual cast in SQL code.
	/// </summary>
	[Serializable]
	public class TransparentCastFunction : CastFunction
	{
		protected override bool CastingIsRequired(string sqlType)
		{
			return false;
		}
	}
}
