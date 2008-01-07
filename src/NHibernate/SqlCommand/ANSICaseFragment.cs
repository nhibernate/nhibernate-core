using System.Collections.Generic;
using System.Text;

namespace NHibernate.SqlCommand
{
	/// <summary>An ANSI SQL CASE expression.
	/// <code>case when ... then ... end as ...</code>
	/// </summary>
	/// <remarks>This class looks StringHelper.SqlParameter safe...</remarks>
	public class ANSICaseFragment : CaseFragment
	{
		public ANSICaseFragment(Dialect.Dialect dialect) : base(dialect) {}

		public override string ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10)
				.Append("case");

			foreach (KeyValuePair<string, string> me in cases)
			{
				buf.Append(" when ").Append(me.Key).Append(" is not null then ").Append(me.Value);
			}
			buf.Append(" end");

			if (returnColumnName != null)
			{
				buf.Append(" as ").Append(returnColumnName);
			}

			return buf.ToString();
		}
	}
}