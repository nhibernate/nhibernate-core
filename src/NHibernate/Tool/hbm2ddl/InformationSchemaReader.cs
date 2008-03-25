namespace NHibernate.Tool.hbm2ddl
{
	using System;
	using System.Data;
	using System.Data.Common;

	public class InformationSchemaReader : ISchemaReader
	{
		private readonly IDbConnection connection;

		public InformationSchemaReader(IDbConnection connection)
		{
			this.connection = connection;
		}

		public DataTable GetTables(string schema, string name, string[] types)
		{
			return
				ExecuteQuery(
					@"SELECT
					TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE 
					FROM INFORMATION_SCHEMA.TABLES"
					);
		}

		public DataTable GetColumns(string schema, string name)
		{
			return ExecuteQuery(
				@"select 
					COLUMN_NAME, IS_NULLABLE, DATA_TYPE, NUMERIC_PRECISION, CHARACTER_MAXIMUM_LENGTH 
					from information_schema.columns
					where table_schema = @schema and table_name = @name",
				"schema", schema, "name", name
				);
		}

		public DataTable GetIndexInfo(string schema, string name)
		{
			//TODO: can't think of a way to do this in a portable manner
			return new DataTable();
		}

		public DataTable GetForeignKeys(string schema, string name)
		{
			return ExecuteQuery(
				@"SELECT 
					CONSTRAINT_NAME 
					FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
					WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name AND CONSTRAINT_TYPE = 'FOREIGN KEY'",
				"schema", schema, "name", name);
		}

		public DataTable GetIndexColumns(string schema, string name, string constraint)
		{
			return ExecuteQuery(
				@"SELECT 
					COLUMN_NAME FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
					WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @name AND CONSTRAINT_NAME = @constraint",
				"schema", schema, "name", name, "constraint", constraint);
		}

		/// <summary>
		/// Executes a query and returns a datatable. The parameters array is used
		/// in the following fashion ExecuteQuery("select @id", "id", 15);
		/// </summary>
		protected DataTable ExecuteQuery(string query, params object[] parameters)
		{
			using (IDbCommand cmd = connection.CreateCommand())
			{
				cmd.CommandText = query;
				for (int i = 0; i < parameters.Length; i += 2) //NOTE, this always jumps by two
				{
					IDbDataParameter parameter = cmd.CreateParameter();
					parameter.ParameterName = (string) parameters[i];
					parameter.Value = parameters[i + 1];
					cmd.Parameters.Add(parameter);
				}
				using (IDataReader reader = cmd.ExecuteReader())
				{
					return new DataTableFromDataReaderAdapter().GetData(reader);
				}
			}
		}

		private class DataTableFromDataReaderAdapter : DbDataAdapter
		{
			public DataTable GetData(IDataReader reader)
			{
				DataTable dt = new DataTable();
				Fill(dt, reader);
				return dt;
			}
		}
	}
}