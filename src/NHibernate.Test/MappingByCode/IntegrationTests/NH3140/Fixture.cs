using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3140
{
	public class Fixture
	{
		[TestCase("DifferentFromBar")]
		[TestCase("Bar")]
		public void ExplicitColumnNameIsAlwaysMapped(string columnName)
		{
			var mapper = new ModelMapper();
			mapper.Class<Foo>(cm => cm.Bag(x => x.Bars,
			 							   bpm => { },
										   cer => cer.ManyToMany(mtmm => mtmm.Column(columnName))));
			mapper.Class<Bar>(cm => cm.Id(x => x.Id));
			var mapping = mapper.CompileMappingFor(new[] { typeof(Foo), typeof(Bar) });
			var hbmClass = mapping.RootClasses.Single(x => x.Name == "Foo");
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();
			var hbmManyToMany = (HbmManyToMany)hbmBag.ElementRelationship;
			Assert.AreEqual(columnName, hbmManyToMany.column);
		}
	}
}