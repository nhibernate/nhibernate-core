using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class IdBagMappingTest
	{
		private class Animal
		{
			public int Id { get; set; }
			private ICollection<Animal> children;
			public ICollection<Animal> Children
			{
				get { return children; }
			}
		}

		[Test]
		public void WhenIdBagWithManyToManyThenMapIt()
		{
			var mapper = new ModelMapper();
			mapper.Class<Animal>(map =>
													 {
														 map.Id(x => x.Id, idmap => { });
														 map.IdBag(x => x.Children, bag => { }, rel=> rel.ManyToMany());
													 });
			var hbmMapping = mapper.CompileMappingFor(new[]{ typeof(Animal)});
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmIdbag = hbmClass.Properties.OfType<HbmIdbag>().SingleOrDefault();
			Assert.That(hbmIdbag, Is.Not.Null);
			Assert.That(hbmIdbag.ElementRelationship, Is.InstanceOf<HbmManyToMany>());
		}

		[Test]
		public void WhenIdBagWithOneToManyThenThrow()
		{
			var mapper = new ModelMapper();
			mapper.Class<Animal>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.IdBag(x => x.Children, bag => { }, rel => rel.OneToMany());
			});
			Assert.That(() => mapper.CompileMappingFor(new[] { typeof(Animal) }), Throws.TypeOf<NotSupportedException>());
		}
	}
}