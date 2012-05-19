using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents part of an SQL <c>SELECT</c> clause
	/// </summary>
	public class SelectFragment
	{
		private string suffix;
		private IList<string> columns = new List<string>();
		private IList<string> columnAliases = new List<string>();
		private Dialect.Dialect dialect;
		private string[] usedAliases;
		private string extraSelectList;

		public SelectFragment(Dialect.Dialect d)
		{
			this.dialect = d;
		}

		public SelectFragment SetUsedAliases(string[] usedAliases)
		{
			this.usedAliases = usedAliases;
			return this;
		}

		public SelectFragment SetSuffix(string suffix)
		{
			this.suffix = suffix;
			return this;
		}

		public SelectFragment AddColumn(string columnName)
		{
			AddColumn(null, columnName);
			return this;
		}

		public SelectFragment AddColumns(string[] columnNames)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				AddColumn(columnNames[i]);
			}
			return this;
		}

		public SelectFragment AddColumn(string tableAlias, string columnName)
		{
			return AddColumn(tableAlias, columnName, columnName);
		}

		public SelectFragment AddColumn(string tableAlias, string columnName, string columnAlias)
		{
			if (string.IsNullOrEmpty(tableAlias))
			{
				columns.Add(columnName);
			}
			else
			{
				columns.Add(tableAlias + StringHelper.Dot + columnName);
			}

			columnAliases.Add(columnAlias);
			return this;
		}

		public SelectFragment AddColumns(string tableAlias, string[] columnNames)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (columnNames[i] != null)
					AddColumn(tableAlias, columnNames[i]);
			}
			return this;
		}

		public SelectFragment AddColumns(string tableAlias, string[] columnNames, string[] columnAliases)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (columnNames[i] != null)
					AddColumn(tableAlias, columnNames[i], columnAliases[i]);
			}
			return this;
		}

		public SelectFragment AddFormulas(string tableAlias, string[] formulas, string[] formulaAliases)
		{
			for (int i = 0; i < formulas.Length; i++)
			{
				if (formulas[i] != null)
					AddFormula(tableAlias, formulas[i], formulaAliases[i]);
			}

			return this;
		}

		public SelectFragment AddFormula(string tableAlias, string formula, string formulaAlias)
		{
			AddColumn(
				null,
				StringHelper.Replace(formula, Template.Placeholder, tableAlias),
				formulaAlias);

			return this;
		}

		/// <summary>
		/// Equivalent to ToSqlStringFragment.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// In H3, it is called ToFragmentString(). It appears to be 
		/// functionally equivalent as ToSqlStringFragment() here.
		/// </remarks>
		public string ToFragmentString()
		{
			return ToSqlStringFragment();
		}

		public string ToSqlStringFragment()
		{
			// this preserves the way this existing method works now.
			return ToSqlStringFragment(true);
		}

		public string ToSqlStringFragment(bool includeLeadingComma)
		{
			StringBuilder buf = new StringBuilder(columns.Count * 10);
			HashSet<string> columnsUnique =
				usedAliases != null ? new HashSet<string>(usedAliases) : new HashSet<string>();

			bool found = false;
			for (int i = 0; i < columns.Count; i++)
			{
				string col = columns[i];
				string columnAlias = columnAliases[i];

				if (columnsUnique.Add(columnAlias))
				{
					if (found || includeLeadingComma)
					{
						buf.Append(StringHelper.CommaSpace);
					}

					buf.Append(col)
						.Append(" as ")
						.Append(new Alias(suffix).ToAliasString(columnAlias, dialect));

					// Set the flag for the next time
					found = true;
				}
			}

			if (extraSelectList != null)
			{
				if (found || includeLeadingComma)
				{
					buf.Append(StringHelper.CommaSpace);
				}

				buf.Append(extraSelectList);
			}

			return buf.ToString();
		}

		public SelectFragment SetExtraSelectList(string extraSelectList)
		{
			this.extraSelectList = extraSelectList;
			return this;
		}

		public SelectFragment SetExtraSelectList(CaseFragment caseFragment, string fragmentAlias)
		{
			SetExtraSelectList(caseFragment.SetReturnColumnName(fragmentAlias, suffix).ToSqlStringFragment());
			return this;
		}
	}
}
