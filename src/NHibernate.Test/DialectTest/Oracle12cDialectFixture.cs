using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Id.Insert;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest
{
	public class IdentityEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	[TestFixture]
	public class oracle12cDialectFixture
	{
		[Test]
		public void GetLimitString()
		{
			var d = new Oracle12cDialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"select distinct c.Contact_Id as Contact1_19_0_, c.Rating as Rating2_19_0_, c.Last_Name as Last_Name3_19_0, c.First_Name as First_Name4_19_0 from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT fish.id FROM fish OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id FROM fish fish_ OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT DISTINCT fish_.id as ixx9_ FROM fish fish_ OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish ORDER BY name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish ORDER BY name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT fish.id, fish.name FROM fish ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT fish.id, fish.name FROM fish ORDER BY name DESC OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish LEFT JOIN (SELECT * FROM meat ORDER BY weight) AS t ORDER BY name DESC OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT *, (SELECT COUNT(1) FROM fowl WHERE fish_id = fish.id) AS some_count FROM fish OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT * FROM fish WHERE scales = ", Parameter.Placeholder), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM fish WHERE scales = ? OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());

			str = d.GetLimitString(new SqlString("SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name)"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT f.Type, COUNT(DISTINCT f.Name) AS Name FROM Fish f GROUP BY f.Type ORDER BY COUNT(DISTINCT f.Name) OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void GetLimitStringWithInnerOrder()
		{
			var d = new Oracle12cDialect();

			var str = d.GetLimitString(new SqlString("SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name"), new SqlString("111"), new SqlString("222"));
			Assert.AreEqual(
				"SELECT * FROM A LEFT JOIN (SELECT top 7 * FROM B ORDER BY name) AS B on A.Name = B.Name OFFSET 111 ROWS FETCH FIRST 222 ROWS ONLY",
				str.ToString());
		}

		[Test]
		public void OnlyOffsetLimit()
		{
			var d = new Oracle12cDialect();

			SqlString str = d.GetLimitString(new SqlString("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name"), null, new SqlString("10"));
			Assert.That(str.ToString(), Is.EqualTo("select distinct c.Contact_Id as Contact1_19_0_, c._Rating as Rating2_19_0_ from dbo.Contact c where COALESCE(c.Rating, 0) > 0 order by c.Rating desc , c.Last_Name , c.First_Name FETCH FIRST 10 ROWS ONLY"));
		}

		[Test]
		public void GetLimitStringWithSqlComments()
		{
			var d = new Oracle12cDialect();
			var limitSqlQuery = d.GetLimitString(new SqlString(" /* criteria query */ SELECT p from lcdtm"), null, new SqlString("2"));
			Assert.That(limitSqlQuery, Is.Not.Null);
			Assert.That(limitSqlQuery.ToString(), Is.EqualTo(" /* criteria query */ SELECT p from lcdtm FETCH FIRST 2 ROWS ONLY"));
		}

		[Test]
		public void NativeGeneratorIsSequence()
		{
			var d = new Oracle12cDialect();

			Assert.That(d.NativeIdentifierGeneratorClass, Is.EqualTo(typeof(SequenceGenerator)), "NativeIdentifierGeneratorClass");
			Assert.That(
				IdentifierGeneratorFactory.GetIdentifierGeneratorClass("native", d),
				Is.EqualTo(typeof(SequenceGenerator)),
				"native generator");
		}

		[Test]
		public void IdentityGeneratorIsIdentity()
		{
			var d = new Oracle12cDialect();

			Assert.That(d.SupportsIdentityColumns, Is.True, "SupportsIdentityColumns");
			Assert.That(d.SupportsInsertSelectIdentity, Is.False, "SupportsInsertSelectIdentity");
			Assert.That(d.SupportsIdentifierOutParameter, Is.True, "SupportsIdentifierOutParameter");
			Assert.That(d.HasDataTypeInIdentityColumn, Is.True, "HasDataTypeInIdentityColumn");
			Assert.That(d.IdentityStyleIdentifierGeneratorClass, Is.EqualTo(typeof(IdentityGenerator)), "IdentityStyleIdentifierGeneratorClass");
			Assert.That(
				IdentifierGeneratorFactory.GetIdentifierGeneratorClass("identity", d),
				Is.EqualTo(typeof(IdentityGenerator)),
				"identity generator");
		}

		[Test]
		public void IdentityGeneratorCreatesIdentityColumn()
		{
			var script = GenerateSchemaCreationScript(Generators.Identity);

			Assert.That(
				script,
				Has.One
					.EqualTo(
						"create table IdentityEntity (Id NUMBER(10,0) generated by default on null as identity," +
						" Name VARCHAR2(255), primary key (Id))"));
			Assert.That(script, Has.None.Contains("create sequence"));
		}

		[Test]
		public void NativeGeneratorDoesNotCreateIdentityColumn()
		{
			var script = GenerateSchemaCreationScript(Generators.Native);

			Assert.That(script, Has.None.Contains("identity"));
			Assert.That(script, Has.One.Contains("create sequence"));
		}

		[Test]
		public void IdentityInsertUsesReturningIntoClause()
		{
			var insert = PrepareIdentifierGeneratingInsert();
			insert.AddColumn("Name", NHibernateUtil.String);

			Assert.That(
				insert.ToSqlString().ToString(),
				Is.EqualTo("INSERT INTO IdentityEntity (Id, Name) VALUES (default, ?) returning Id into :nhIdOutParam"));
		}

		[Test]
		public void IdentityInsertWithoutOtherColumns()
		{
			var insert = PrepareIdentifierGeneratingInsert();

			Assert.That(
				insert.ToSqlString().ToString(),
				Is.EqualTo("INSERT INTO IdentityEntity (Id) VALUES (default) returning Id into :nhIdOutParam"));
		}

		private static IdentifierGeneratingInsert PrepareIdentifierGeneratingInsert()
		{
			var factory = Substitute.For<ISessionFactoryImplementor>();
			factory.Dialect.Returns(new Oracle12cDialect());
			var persister = Substitute.For<IPostInsertIdentityPersister>();
			persister.RootTableKeyColumnNames.Returns(new[] { "Id" });
			persister.IdentifierType.Returns(NHibernateUtil.Int32);

			var identifierDelegate =
				new IdentityGenerator().GetInsertGeneratedIdentifierDelegate(persister, factory, false);
			Assert.That(identifierDelegate, Is.InstanceOf<OutputParamReturningDelegate>());

			var insert = identifierDelegate.PrepareIdentifierGeneratingInsert();
			insert.SetTableName("IdentityEntity");
			return insert;
		}

		private static string[] GenerateSchemaCreationScript(IGeneratorDef generator)
		{
			var mapper = new ModelMapper();
			mapper.Class<IdentityEntity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(generator));
					rc.Property(x => x.Name);
				});

			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

			var dialect = new Oracle12cDialect();
			// Oracle dialects register their character and floating point types when configured only.
			dialect.Configure(new Dictionary<string, string>());

			return configuration.GenerateSchemaCreationScript(dialect);
		}
	}
}