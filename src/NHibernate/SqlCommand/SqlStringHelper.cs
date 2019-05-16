using System;
using System.Collections;
using System.Collections.Generic;
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
			int index = sql.LastIndexOfCaseInsensitive(" as ");
			if (index < 0) return sql;
			return sql.Substring(0, index);
		}


		public static bool IsNotEmpty(SqlString str)
		{
			return !IsEmpty(str);
		}

		public static bool IsEmpty(SqlString str)
		{
			return str == null || str.Count == 0;
		}

		internal static SqlString ParametersList(List<Parameter> parameters)
		{
			var parametersCount = parameters.Count;
			if (parametersCount == 0)
			{
				return SqlString.Empty;
			}

			if (parametersCount == 1)
			{
				return new SqlString(parameters[0]);
			}

			var builder = new SqlStringBuilder();
			builder.Add("(");

			builder.Add(parameters[0]);

			for (var index = 1; index < parametersCount; index++)
			{
				builder.Add(", ");
				builder.Add(parameters[index]);
			}

			builder.Add(")");

			return builder.ToSqlString();
		}

		internal static SqlString Repeat(SqlString placeholder, int count, string separator, string[] wrapResult)
		{
			return Repeat(
				placeholder,
				count,
				new SqlString(separator),
				wrapResult == null
					? null
					: new[]
					{
						new SqlString(wrapResult[0]),
						new SqlString(wrapResult[1]),
					});
		}

		internal static SqlString Repeat(SqlString placeholder, int count, SqlString separator, SqlString[] wrapResult)
		{
			if (wrapResult == null)
			{
				if (count == 0)
					return SqlString.Empty;
				if (count == 1)
					return placeholder;
			}

			var builder = new SqlStringBuilder(count * 2 + 1);
			if (wrapResult != null)
			{
				builder.Add(wrapResult[0]);
			}

			if (count > 0)
				builder.Add(placeholder);

			for (int i = 1; i < count; i++)
			{
				builder.Add(separator).Add(placeholder);
			}

			if (wrapResult != null)
			{
				builder.Add(wrapResult[1]);
			}
			return builder.ToSqlString();
		}
	}
}
