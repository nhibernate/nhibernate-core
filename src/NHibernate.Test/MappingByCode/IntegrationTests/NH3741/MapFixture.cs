using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3741
{
	[TestFixture]
	public class MapFixture
	{
		[Test]
		public void TestMapManyToManyGenericCollectionBasedOnInterface()
		{
			var mapper = new ModelMapper();

			mapper.Class<ParentWithInterfaceChild>(c =>
			{
				c.Id(x => x.Id, x => x.Generator(Generators.Identity));
				c.IdBag(x => x.Children, bag => { }, rel => rel.ManyToMany());
			});

			mapper.Class<Child>(c =>
			{
				c.Id(x => x.Id, x => x.Generator(Generators.Identity));
				c.Property(x => x.Description);
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(ParentWithInterfaceChild), typeof(Child) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmIdbag = hbmClass.Properties.OfType<HbmIdbag>().SingleOrDefault();

			Assert.That(hbmIdbag, Is.Not.Null);
			Assert.That(hbmIdbag.ElementRelationship, Is.InstanceOf<HbmManyToMany>());
		}

		[Test]
		public void TestMapManyToManyGenericCollectionBasedOnEntity()
		{
			var mapper = new ModelMapper();

			mapper.Class<ParentWithEntityChild>(c =>
			{
				c.Id(x => x.Id, x => x.Generator(Generators.Identity));
				c.IdBag(x => x.Children, bag => { }, rel => rel.ManyToMany());
			});

			mapper.Class<Child>(c =>
			{
				c.Id(x => x.Id, x => x.Generator(Generators.Identity));
				c.Property(x => x.Description);
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(ParentWithEntityChild), typeof(Child) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmIdbag = hbmClass.Properties.OfType<HbmIdbag>().SingleOrDefault();

			Assert.That(hbmIdbag, Is.Not.Null);
			Assert.That(hbmIdbag.ElementRelationship, Is.InstanceOf<HbmManyToMany>());
		}

		[Test]
		public void TestMapManyToManyGenericCollectionBasedOnItem()
		{
			var mapper = new ModelMapper();

			mapper.Class<ParentWithItemChild>(c =>
			{
				c.Id(x => x.Id, x => x.Generator(Generators.Identity));
				c.IdBag(x => x.Children, bag => { }, rel => rel.ManyToMany());
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(ParentWithItemChild) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmIdbag = hbmClass.Properties.OfType<HbmIdbag>().SingleOrDefault();

			Assert.That(hbmIdbag, Is.Not.Null);
			Assert.That(hbmIdbag.ElementRelationship, Is.InstanceOf<HbmManyToMany>());
		}
	}
}
