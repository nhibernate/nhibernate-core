using System.Collections;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	/// <remarks>This class looks StringHelper.SqlParameter safe...</remarks>
	public class ANSICaseFragment : CaseFragment
	{
		private readonly Dialect.Dialect dialect;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		public ANSICaseFragment(Dialect.Dialect dialect)
		{
			this.dialect = dialect;
		}

		private string returnColumnName;

		private readonly IList cases = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnColumnName"></param>
		/// <returns></returns>
		public override CaseFragment SetReturnColumnName(string returnColumnName)
		{
			this.returnColumnName = returnColumnName;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnColumnName"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public override CaseFragment SetReturnColumnName(string returnColumnName, string suffix)
		{
			return SetReturnColumnName(new Alias(suffix).ToAliasString(returnColumnName, dialect));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columnName"></param>
		/// <param name="columnValue"></param>
		/// <returns></returns>
		public override CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue)
		{
			string key = alias + StringHelper.Dot + columnName + " is not null";

			cases.Add(" when " + key + " then " + columnValue);
			return this;
		}

		/// <summary></summary>
		public override string ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10);

			buf.Append("case");

			for (int i = 0; i < cases.Count; i++)
			{
				buf.Append(cases[i]);
			}

			buf.Append(" end");

			if (returnColumnName != null)
			{
				buf.Append(" as ")
					.Append(returnColumnName);
			}

			return buf.ToString();
		}
	}
}