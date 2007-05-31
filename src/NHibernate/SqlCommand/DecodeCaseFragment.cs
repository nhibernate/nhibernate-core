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
		private IDictionary cases = new SequencedHashMap();

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
			cases.Add(key, columnValue);
			return this;
		}

		/// <summary></summary>
		public override string ToSqlStringFragment()
		{
			StringBuilder buf = new StringBuilder(cases.Count * 15 + 10)
				.Append("decode(");

			int number = 0;
			foreach (DictionaryEntry de in cases)
			{
				if (number < cases.Count - 1)
				{
					buf.Append(", ")
						.Append(de.Key)
						.Append(", ")
						.Append(de.Value);
				}
				else
				{
					buf.Insert(7, de.Key)
						.Append(", ")
						.Append(de.Value);
				}
				number++;
			}

			buf.Append(')');
			return buf.ToString();
		}
	}
}