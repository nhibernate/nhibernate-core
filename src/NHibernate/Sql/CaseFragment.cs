using System;

namespace NHibernate.Sql {
	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	public abstract class CaseFragment {
		public abstract CaseFragment SetReturnColumnName(string returnColumnName);
		public abstract CaseFragment SetReturnColumnName(string returnColumnName, string suffix);
		public abstract CaseFragment AddWhenColumnNotNull(string alias, string columnName, string value);
		public abstract string ToFragmentString();
	}
}
