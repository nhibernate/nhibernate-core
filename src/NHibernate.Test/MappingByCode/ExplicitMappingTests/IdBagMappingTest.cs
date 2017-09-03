using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	[TestFixture]
	public class IdBagMappingTest
	{
		private class Animal
		{
			public int Id { get; set; }
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private ICollection<Animal> children;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
