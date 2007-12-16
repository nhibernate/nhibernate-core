using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// The default <cref name="INamingStrategy"/>
	/// </summary>
	/// <remarks>See <cref name="ImprovedNamingStrategy"/> for a better alternative</remarks>
	public class DefaultNamingStrategy : INamingStrategy
	{
		/// <summary>
		/// The singleton instance
		/// </summary>
		public static readonly INamingStrategy Instance = new DefaultNamingStrategy();

		private DefaultNamingStrategy()
		{
		}

		#region INamingStrategy Members

		/// <summary>
		/// Return the unqualified class name
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public string ClassToTableName(string className)
		{
			return StringHelper.Unqualify(className);
		}

		/// <summary>
		/// Return the unqualified property name
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string PropertyToColumnName(string propertyName)
		{
			return StringHelper.Unqualify(propertyName);
		}

		/// <summary>
		/// Return the argument
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string TableName(string tableName)
		{
			return tableName;
		}

		/// <summary>
		/// Return the argument
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public string ColumnName(string columnName)
		{
			return columnName;
		}

		/// <summary>
		/// Return the unqualified property name
		/// </summary>
		/// <param name="className"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string PropertyToTableName(string className, string propertyName)
		{
			return StringHelper.Unqualify(propertyName);
		}

		public string LogicalColumnName(string columnName, string propertyName)
		{
			return StringHelper.IsNotEmpty(columnName) ? columnName : StringHelper.Unqualify(propertyName);
		}

		#endregion
	}
}