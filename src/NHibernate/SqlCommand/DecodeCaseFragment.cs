using System.Collections;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL decode(pkvalue, key1, 1, key2, 2, ..., 0)
	/// </summary>
	public class DecodeCaseFragment : CaseFragment
	{
		private Dialect.Dialect dialect;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		public DecodeCaseFragment(Dialect.Dialect dialect)
		{
			this.dialect = dialect;
		}

		private string returnColumnName;
		private IList cases = new ArrayList();

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
			string key = alias + StringHelper.Dot + columnName;

			if (columnValue.Equals("0"))
			{
				cases.Insert(0, key);
			}
			else
			{
				cases.Add(", " + key + ", " + columnValue);
			}

			return this;
		}

		/// <summary></summary>
		public override SqlString ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10);

			buf.Append("decode (");

			for (int i = 0; i < cases.Count; i++)
			{
				buf.Append(cases[i]);
			}

			buf.Append(",0 )");

			if (returnColumnName != null)
			{
				buf.Append(" as ")
					.Append(returnColumnName);
			}

			return new SqlString(buf.ToString());
		}
	}
}