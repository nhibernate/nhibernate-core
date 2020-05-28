using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// A HQL only cast for helping HQL knowing the type. Does not generates any actual cast in SQL code.
	/// </summary>
	[Serializable]
	public class TransparentCastFunction : CastFunction
	{
		// Since v5.3
		[Obsolete("This method has no usages and will be removed in a future version")]
		protected override bool CastingIsRequired(string sqlType)
		{
			return false;
		}

		protected override SqlString Render(IList args, string sqlType, ISessionFactoryImplementor factory)
		{
			return new SqlString("(", args[0], ")");
		}
	}
}
