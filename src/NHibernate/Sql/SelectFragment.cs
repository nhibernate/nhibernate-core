using System;
using System.Text;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Sql {
	
	/// <summary>
	/// Represents part of an SQL <c>SELECT</c> clause
	/// </summary>
	public class SelectFragment {
		private string suffix;
		private IList columns = new ArrayList(); //TODO: use specialized string collection?
		private IList aliases = new ArrayList();
		private IList columnAliases = new ArrayList();

		public SelectFragment SetSuffix(string suffix) {
			this.suffix = suffix;
			return this;
		}

		public SelectFragment AddColumn(string columnName) {
			AddColumn(null, columnName);
			return this;
		}

		public SelectFragment AddColumns(string[] columnNames) {
			for (int i=0; i<columnNames.Length; i++) AddColumn(columnNames[i]);
			return this;
		}

		public SelectFragment AddColumn(string tableAlias, string columnName) {
			return AddColumn(tableAlias, columnName, columnName);
		}

		public SelectFragment AddColumn(string tableAlias, string columnName, string columnAlias) {
			columns.Add(columnName);
			aliases.Add(tableAlias);
			columnAliases.Add(columnAlias);
			return this;
		}

		public SelectFragment AddColumns(string tableAlias, string[] columnNames) {
			for (int i=0; i<columnNames.Length; i++) AddColumn(tableAlias, columnNames[i]);
			return this;
		}

		public SelectFragment AddColumns(string tableAlias, string[] columnNames, string[] columnAliases) {
			for (int i=0; i<columnNames.Length; i++)
				AddColumn(tableAlias, columnNames[i], columnAliases[i]);
			return this;
		}
		
		public string ToFragmentString() {
			StringBuilder buf = new StringBuilder(columns.Count * 10);

			for (int i=0; i<columns.Count; i++) {
				string col = columns[i] as string;
				buf.Append(StringHelper.CommaSpace);
				string alias = aliases[i] as string;
				if (alias!=null) buf.Append(alias).Append(StringHelper.Dot);
				string columnAlias = columnAliases[i] as string;
				buf.Append(col)
					.Append(" as ")
					.Append( new Alias(suffix).ToAliasString(columnAlias) );
			}
			return buf.ToString();
		}
	}
}
