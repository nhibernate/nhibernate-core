using System.Collections.Generic;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>An Oracle-style DECODE function. </summary>
	/// <example>
	/// <code>decode(pkvalue, key1, 1, key2, 2, ..., 0)</code>
	/// </example>
	public class DecodeCaseFragment : CaseFragment
	{
		public DecodeCaseFragment(Dialect.Dialect dialect) : base(dialect) {}

		/// <summary></summary>
		public override string ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10)
				.Append("decode(");

			int number = 0;
			foreach (KeyValuePair<string, string> de in cases)
			{
				if (number < cases.Count - 1)
				{
					buf.Append(StringHelper.CommaSpace)
						.Append(de.Key)
						.Append(StringHelper.CommaSpace)
						.Append(de.Value);
				}
				else
				{
					buf.Insert(7, de.Key)
						.Append(StringHelper.CommaSpace)
						.Append(de.Value);
				}
				number++;
			}
			buf.Append(StringHelper.ClosedParen);

			if (!string.IsNullOrEmpty(returnColumnName))
			{
				buf.Append(" as ").Append(returnColumnName);
			}

			return buf.ToString();
		}
	}
}