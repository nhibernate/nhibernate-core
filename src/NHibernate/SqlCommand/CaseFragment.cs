using System;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	public abstract class CaseFragment 
	{
		public abstract CaseFragment SetReturnColumnName(string returnColumnName);
		public abstract CaseFragment SetReturnColumnName(string returnColumnName, string suffix);
		public abstract CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue);
		public abstract SqlString ToSqlStringFragment();
	}

	
}
