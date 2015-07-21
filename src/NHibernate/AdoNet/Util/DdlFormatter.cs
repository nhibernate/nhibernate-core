using System.Collections.Generic;
using System.Text;
using NHibernate.Util;

namespace NHibernate.AdoNet.Util
{
	public class DdlFormatter: IFormatter
	{
		private const string Indent1 = "\n    ";
		private const string Indent2 = "\n      ";
		private const string Indent3 = "\n        ";

		/// <summary> Format an SQL statement using simple rules:
		/// a) Insert newline after each comma;
		/// b) Indent three spaces after each inserted newline;
		/// If the statement contains single/double quotes return unchanged,
		/// it is too complex and could be broken by simple formatting.
		/// </summary>
		public virtual string Format(string sql)
		{
			if (sql.ToLowerInvariant().StartsWith("create table"))
			{
				return FormatCreateTable(sql);
			}
			else if (sql.ToLowerInvariant().StartsWith("alter table"))
			{
				return FormatAlterTable(sql);
			}
			else if (sql.ToLowerInvariant().StartsWith("comment on"))
			{
				return FormatCommentOn(sql);
			}
			else
			{
				return Indent1 + sql;
			}
		}

		protected virtual string FormatCommentOn(string sql)
		{
			StringBuilder result = new StringBuilder(60).Append(Indent1);
			IEnumerator<string> tokens = (new StringTokenizer(sql, " '[]\"", true)).GetEnumerator();

			bool quoted = false;
			while (tokens.MoveNext())
			{
				string token = tokens.Current;
				result.Append(token);
				if (IsQuote(token))
				{
					quoted = !quoted;
				}
				else if (!quoted)
				{
					if ("is".Equals(token))
					{
						result.Append(Indent2);
					}
				}
			}

			return result.ToString();
		}

		protected virtual string FormatAlterTable(string sql)
		{
			StringBuilder result = new StringBuilder(60).Append(Indent1);
			IEnumerator<string> tokens = (new StringTokenizer(sql, " (,)'[]\"", true)).GetEnumerator();

			bool quoted = false;
			while (tokens.MoveNext())
			{
				string token = tokens.Current;
				if (IsQuote(token))
				{
					quoted = !quoted;
				}
				else if (!quoted)
				{
					if (IsBreak(token))
					{
						result.Append(Indent3);
					}
				}
				result.Append(token);
			}

			return result.ToString();
		}

		protected virtual string FormatCreateTable(string sql)
		{
			StringBuilder result = new StringBuilder(60).Append(Indent1);
			IEnumerator<string> tokens = (new StringTokenizer(sql, "(,)'[]\"", true)).GetEnumerator();

			int depth = 0;
			bool quoted = false;
			while (tokens.MoveNext())
			{
				string token = tokens.Current;
				if (IsQuote(token))
				{
					quoted = !quoted;
					result.Append(token);
				}
				else if (quoted)
				{
					result.Append(token);
				}
				else
				{
					if (")".Equals(token))
					{
						depth--;
						if (depth == 0)
						{
							result.Append(Indent1);
						}
					}
					result.Append(token);
					if (",".Equals(token) && depth == 1)
					{
						result.Append(Indent2);
					}
					if ("(".Equals(token))
					{
						depth++;
						if (depth == 1)
						{
							result.Append(Indent3);
						}
					}
				}
			}

			return result.ToString();
		}

		private static bool IsBreak(string token)
		{
			return "drop".Equals(token) || "add".Equals(token) || "references".Equals(token) || "foreign".Equals(token)
			       || "on".Equals(token);
		}

		private static bool IsQuote(string token)
		{
			return "\"".Equals(token) || "`".Equals(token) || "]".Equals(token) || "[".Equals(token) || "'".Equals(token);
		}
	}
}