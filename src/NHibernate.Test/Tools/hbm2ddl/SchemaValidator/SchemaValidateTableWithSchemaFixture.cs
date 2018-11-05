using System;
using NHibernate.Dialect;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaValidator
{
	[TestFixture]
	public class SchemaValidateTableWithSchemaFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "Tools.hbm2ddl.SchemaValidator.VersionWithSchema.hbm.xml" };

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			switch (Dialect)
			{
				case MsSql2000Dialect _:
				case PostgreSQLDialect _:
				case SQLiteDialect _:
				case MsSqlCeDialect _:
					return true;
				default:
					// Firebird does not support schema. Its current dialect leave table name being schema prefixed,
					// which causes SQL parse errors. It has a "create schema" command but instead creates a new
					// database.
					// MySql does not truly support schema. Its current dialect leave table name being schema prefixed,
					// which is interpreted as a database name. It has a "create schema" command but instead creates
					// a new database.
					// Oracle tightly bounds schema to users.
					return false;
			}
		}

		protected override void CreateSchema()
		{
			switch (Dialect)
			{
				case MsSql2000Dialect _:
				case PostgreSQLDialect _:
					// Must handle the schema manually: mapped database-objects are handled too late.
					var cnx = Sfi.ConnectionProvider.GetConnection();
					try
					{
						using (var cmd = cnx.CreateCommand())
						{
							cmd.CommandText = "create schema Test";
							cmd.ExecuteNonQuery();
						}
					}
					catch (Exception ex)
					{
						// Unfortunateley Assert.Warn and Console.WriteLine at this place seems to be ignored in Rider
						// viewer.
						Assert.Warn("Creating the schema failed, assuming it already exists. {0}", ex);
						Console.WriteLine("Creating the schema failed, assuming it already exists.");
						Console.WriteLine(ex);
					}
					finally
					{
						Sfi.ConnectionProvider.CloseConnection(cnx);
					}
					break;
			}
			base.CreateSchema();
		}

		protected override void DropSchema()
		{
			// SQL-Server does not need this call, but Postgres does not accept dropping a schema carrying objects.
			base.DropSchema();

			switch (Dialect)
			{
				case MsSql2000Dialect _:
				case PostgreSQLDialect _:
					var cnx = Sfi.ConnectionProvider.GetConnection();
					try
					{
						using (var cmd = cnx.CreateCommand())
						{
							cmd.CommandText = "drop schema Test";
							cmd.ExecuteNonQuery();
						}
					}
					finally
					{
						Sfi.ConnectionProvider.CloseConnection(cnx);
					}
					break;
			}
		}

		[Test]
		public void ShouldVerify()
		{
			var validator = new Tool.hbm2ddl.SchemaValidator(cfg);
			try
			{
				validator.Validate();
			}
			catch (SchemaValidationException sve)
			{
				Assert.Fail("Validation failed: {0}.\n{1}", StringHelper.CollectionToString(sve.ValidationErrors), sve);
			}
		}
	}
}
