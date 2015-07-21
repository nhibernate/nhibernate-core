using System;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Given an SQL SELECT statement, parse it to extract clauses starting with
	/// <c>FROM</c>, up to and not including <c>ORDER BY</c> (known collectively
	/// as a subselect clause).
	/// </summary>
	public class SubselectClauseExtractor
	{
		/* 
		 * NH TODO: this implementation will break, for MsSQL2005Dialect, a when the query is an HQL with skip/take because the last "ORDER BY" is there for pagination.
		 * Because HQL skip/take are new features, we hope nobody will use it in conjuction with subselect fetching at least until MS-SQL will release a more modern
		 * syntax for pagination.
		*/
		private const string FromClauseToken = " from ";
		private const string OrderByToken = "order by";

		private int lastOrderByIndex;
		private int lastOrderByPartIndex;
		private int parenNestCount;
		private SqlString sql;

		/// <summary>
		/// Contains the subselect clause as it is being built.
		/// </summary>
		private SqlStringBuilder builder;

		/// <summary>
		/// Initializes a new instance of the <see cref="SubselectClauseExtractor"/> class.
		/// </summary>
		/// <param name="sql">The <see cref="SqlString" /> to extract the subselect clause from.</param>
		public SubselectClauseExtractor(SqlString sql)
		{
			builder = new SqlStringBuilder(sql.Count);
			this.sql = sql;
			lastOrderByIndex = -1;
			lastOrderByPartIndex = -1;
		}

		/// <summary>
		/// Looks for a <c>FROM</c> clause in the <paramref name="part"/>
		/// and adds the clause to the result if found.
		/// </summary>
		/// <param name="part">A <see cref="String"/> or a <see cref="Parameter"/>.</param>
		/// <returns><see langword="true" /> if the part contained a <c>FROM</c> clause,
		/// <see langword="false" /> otherwise.</returns>
		private bool ProcessPartBeforeFrom(object part)
		{
			string partString = part as string;

			if (partString == null)
			{
				return false;
			}

			int fromStart = FindFromClauseInPart(partString);
			if (fromStart >= 0)
			{
				AddPart(partString.Substring(fromStart));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the subselect clause of the statement
		/// being processed.
		/// </summary>
		/// <returns>An <see cref="SqlString" /> containing
		/// the subselect clause of the original <c>SELECT</c>
		/// statement.</returns>
		public SqlString GetSqlString()
		{
			IEnumerator partEnumerator = sql.GetEnumerator();
			parenNestCount = 0;
			// Process the parts until FROM is found
			while (partEnumerator.MoveNext())
			{
				object part = partEnumerator.Current;
				if (ProcessPartBeforeFrom(part))
				{
					break;
				}
			}

			// Process the rest
			while (partEnumerator.MoveNext())
			{
				AddPart(partEnumerator.Current);
			}

			RemoveLastOrderByClause();

			return builder.ToSqlString();
		}

		public static bool HasOrderBy(SqlString subselect)
		{
			var extractor = new SubselectClauseExtractor(subselect);
			extractor.GetSqlString();
			return extractor.lastOrderByPartIndex >= 0;
		}

		private int FindFromClauseInPart(string part)
		{
			int afterLastClosingParenIndex = 0;
			int fromIndex = StringHelper.IndexOfCaseInsensitive(part, FromClauseToken);
			
			for (int i = 0; i < part.Length; i++)
			{
				if (parenNestCount == 0 && i > fromIndex)
				{
					break;
				}

				if (part[i] == '(')
				{
					++parenNestCount;
				}
				else if (part[i] == ')')
				{
					--parenNestCount;
					if (parenNestCount != 0)
					{
						continue;
					}

					afterLastClosingParenIndex = i + 1;
					fromIndex = StringHelper.IndexOfCaseInsensitive(part, FromClauseToken, afterLastClosingParenIndex);
				}
			}

			if (afterLastClosingParenIndex == 0)
			{
				fromIndex = StringHelper.IndexOfCaseInsensitive(part, FromClauseToken);
			}

			if(parenNestCount > 0)
				return -1;

			return fromIndex;
		}

		private void AddPart(object part)
		{
			builder.AddObject(part);
			CheckLastPartForOrderByClause();
		}

		private void CheckLastPartForOrderByClause()
		{
			object part = builder[builder.Count - 1];
			if (part == Parameter.Placeholder)
			{
				return;
			}

			string partString = part as string;
			int index = StringHelper.LastIndexOfCaseInsensitive(partString, OrderByToken);
			if (index >= 0)
			{
				lastOrderByPartIndex = builder.Count - 1;
				lastOrderByIndex = index;
			}
			
			IgnoreOrderByInSubselect(partString);
		}

		private void IgnoreOrderByInSubselect(string partString)
		{
			int index;
			index = StringHelper.LastIndexOfCaseInsensitive(partString, ")");
			if (index >= 0 && ParenIsAfterLastOrderBy(index))
			{
				lastOrderByPartIndex = -1;
				lastOrderByIndex = -1;
			}
		}

		private bool ParenIsAfterLastOrderBy(int index)
		{
			return (builder.Count - 1) > lastOrderByPartIndex || index > lastOrderByIndex;
		}

		private void RemoveLastOrderByClause()
		{
			if (lastOrderByPartIndex < 0)
			{
				// No ORDER BY clause
				return;
			}

			while (builder.Count > lastOrderByPartIndex + 1)
			{
				builder.RemoveAt(builder.Count - 1);
			}

			string lastPart = builder[builder.Count - 1] as string;
			if (lastPart != null)
			{
				builder[builder.Count - 1] = lastPart.Substring(0, lastOrderByIndex);
			}
		}
	}
}
