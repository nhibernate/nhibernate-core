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

		/// <summary>
		/// Renders the SQL fragment representing the casted expression without actually casting it.
		/// </summary>
		/// <param name="expression">The cast argument.</param>
		/// <param name="sqlType">The SQL type to cast to, ignored for rendering.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>A SQL fragment.</returns>
		protected override SqlString Render(object expression, string sqlType, ISessionFactoryImplementor factory)
		{
			return new SqlString("(", expression, ")");
		}
	}
}
