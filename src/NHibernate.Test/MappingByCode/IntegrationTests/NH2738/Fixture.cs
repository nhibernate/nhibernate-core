using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2738
{
	public enum MyEmptyEnum
	{
		
	}

	public class MyClass
	{
		public virtual int Id { get; set; }
		public MyEmptyEnum MyEmptyEnum { get; set; }
	}
	public class Fixture
	{
		[Test]
		public void WhenMapEmptyEnumThenThrowsExplicitException()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(rc =>
			                      {
															rc.Id(x => x.Id);
															rc.Property(x => x.MyEmptyEnum);
			                      });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			conf.AddMapping(mappings);

			conf.Executing(c => c.BuildSessionFactory()).Throws<MappingException>().And.ValueOf.Message.Should().Contain("MyEmptyEnum");
		}
	}
}