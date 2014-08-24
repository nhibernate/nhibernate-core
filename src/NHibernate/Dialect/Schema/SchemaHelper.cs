using System;
using System.Data;

namespace NHibernate.Dialect.Schema
{
	public static class SchemaHelper
	{

		/// <summary>
		/// Get a value from the DataRow. Multiple alternative column names can be given.
		/// The names are tried in order, and the value from the first present column
		/// is returned.
		/// </summary>
		public static object GetValue(DataRow row, params string[] alternativeColumnNames)
		{
			if (alternativeColumnNames == null)
				throw new ArgumentNullException("alternativeColumnNames");

			if (alternativeColumnNames.Length < 1)
				throw new ArgumentOutOfRangeException("alternativeColumnNames", "At least one name must be given.");

			foreach (var name in alternativeColumnNames)
			{
				if (row.Table.Columns.Contains(name))
					return row[name];
			}

			throw new Exception("None of the alternative column names found in the DataTable.");
		}


		/// <summary>
		/// Get a string value from the DataRow. Multiple alternative column names can be given.
		/// The names are tried in order, and the value from the first present column
		/// is returned.
		/// </summary>
		public static string GetString(DataRow row, params string[] alternativeColumnNames)
		{
			return Convert.ToString(GetValue(row, alternativeColumnNames));
		}
	}
}
