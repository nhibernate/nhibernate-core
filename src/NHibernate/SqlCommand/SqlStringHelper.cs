using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Helper methods for SqlString.
	/// </summary>
	public static class SqlStringHelper
	{

		public static SqlString Join(SqlString separator, IEnumerable objects)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			bool first = true;

			foreach (object obj in objects)
			{
				if (!first)
				{
					buf.Add(separator);
				}

				first = false;
				buf.AddObject(obj);
			}

			return buf.ToSqlString();
		}


		public static SqlString[] Add(SqlString[] x, string sep, SqlString[] y)
		{
			SqlString[] result = new SqlString[x.Length];
			for (int i = 0; i < x.Length; i++)
			{
				result[i] = new SqlString(x[i], sep, y[i]);
			}
			return result;
		}


		public static SqlString RemoveAsAliasesFromSql(SqlString sql)
		{
			return sql.Substring(0, sql.LastIndexOfCaseInsensitive(" as "));
		}


		public static bool IsNotEmpty(SqlString str)
		{
			return !IsEmpty(str);
		}


		public static bool IsEmpty(SqlString str)
		{
			return str == null || str.Count == 0;
		}
	}
}
