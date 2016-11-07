using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
namespace NHibernate.Test.NHSpecificTest.NH2569
{
	public class MyClass
	{
		public virtual int Id { get; set; }
	}

	public class Fixture
	{
		[Test]
		public void WhenMapHiloToDifferentSchemaThanClassThenIdHasTheMappedSchema()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(cm =>
								  {
															cm.Schema("aSchema");
															cm.Id(x => x.Id, idm => idm.Generator(Generators.HighLow, gm => gm.Params(new
																																	  {
																																		table = "hilosequences",
																																		schema="gSchema"
																																	  })));
								  });
			var conf = new Configuration();
			conf.DataBaseIntegration(x=> x.Dialect<MsSql2008Dialect>());
			conf.AddDeserializedMapping(mapper.CompileMappingForAllExplicitlyAddedEntities(), "wholeDomain");

			var mappings = conf.CreateMappings(Dialect.Dialect.GetDialect());
			var pc = mappings.GetClass(typeof(MyClass).FullName);
			Assert.That(((SimpleValue)pc.Identifier).IdentifierGeneratorProperties["schema"], Is.EqualTo("gSchema"));
		}
	}
}