using System.Collections;
using System.Text;

using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// Jet engine doesn't support CASE ... WHEN ... END syntax, but has a proprietary "Switch". 
	/// </summary>
	/// <remarks>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </remarks>
	public class JetCaseFragment : CaseFragment
	{
		private Dialect.Dialect dialect;

		public JetCaseFragment(Dialect.Dialect dialect)
		{
			this.dialect = dialect;
		}

		private string returnColumnName;

		private IList cases = new ArrayList();

		public override CaseFragment SetReturnColumnName(string returnColumnName)
		{
			this.returnColumnName = returnColumnName;
			return this;
		}

		public override CaseFragment SetReturnColumnName(string returnColumnName, string suffix)
		{
			return SetReturnColumnName(new Alias(suffix).ToAliasString(returnColumnName, dialect));
		}

		public override CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue)
		{
			string key = alias + StringHelper.Dot + columnName + " is not null";

			cases.Add(key + ", " + columnValue);
			return this;
		}

		public override SqlString ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10);

			buf.Append("Switch(");

			for (int i = 0; i < cases.Count - 1; i++)
			{
				buf.Append(cases[i]);
				buf.Append(", ");
			}
			buf.Append(cases[cases.Count - 1]);

			buf.Append(" )");

			if (returnColumnName != null)
			{
				buf.Append(" as ")
					.Append(returnColumnName);
			}

			return new SqlString(buf.ToString());
		}
	}
}