using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class ComponetsParentAccessorTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyCompo Compo { get; set; }
			public IEnumerable<MyCompo> Compos { get; set; }
		}

		private class MyCompo
		{
			private MyClass parent;
			public MyClass Parent
			{
				get { return parent; }
			}

			public MyNestedCompo NestedCompo { get; set; }
		}

		private class MyNestedCompo
		{
			private MyCompo owner;
			public MyCompo Owner
			{
				get { return owner; }
			}

			public string Something { get; set; }
		}

		private HbmMapping GetMappingWithParentInCompo()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(x =>
			{
				x.Id(c => c.Id);
				x.Component(c => c.Compo);
				x.Bag(c => c.Compos, cm => { });
			});
			mapper.Component<MyCompo>(x =>
			{
				x.Parent(c => c.Parent);
				x.Component(c => c.NestedCompo);
			});
			mapper.Component<MyNestedCompo>(x =>
			{
				x.Component(c => c.Owner);
				x.Property(c => c.Something);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void WhenMapComoponetWithNestBidirectionalComponentThenMapParentAccessor()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			var hbmMyNestedCompo = hbmMyCompo.Properties.OfType<HbmComponent>().Single();

			Assert.That(hbmMyNestedCompo.Parent.access, Is.StringContaining("camelcase"));
		}

		[Test]
		public void WhenCollectionOfComoponetsWithNestBidirectionalComponentThenMapParentAccessor()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			var hbmMyNestedCompo = hbmMyCompo.Properties.OfType<HbmNestedCompositeElement>().Single();

			Assert.That(hbmMyNestedCompo.Parent.access, Is.StringContaining("camelcase"));
		}

		[Test, Ignore("No fixed yet. When the parent is an entity it should be managed explicitly as explicitly is managed the relation (Parent instead many-to-one)")]
		public void WhenMapComoponetWithParentThenMapParentAccessor()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			Assert.That(hbmMyCompo.Parent.access, Is.StringContaining("camelcase"));
		}

		[Test, Ignore("No fixed yet. When the parent is an entity it should be managed explicitly as explicitly is managed the relation (Parent instead many-to-one)")]
		public void WhenCollectionOfComoponetsWithParentThenMapParentAccessor()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			Assert.That(hbmMyCompo.Parent.access, Is.StringContaining("camelcase"));
		}
	}
}