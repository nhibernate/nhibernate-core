using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class NestedComponetsTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyCompo Compo { get; set; }
			public IEnumerable<MyCompo> Compos { get; set; }
		}

		private class MyCompo
		{
			public MyClass AManyToOne { get; set; }
			public MyNestedCompo NestedCompo { get; set; }
		}

		private class MyNestedCompo
		{
			public MyCompo Owner { get; set; }
			public string Something { get; set; }
		}

		private HbmMapping GetMappingWithManyToOneInCompo()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(x =>
			                      {
			                      	x.Id(c => c.Id);
			                      	x.Component(c => c.Compo);
			                      	x.Bag(c => c.Compos, cm => { });
			                      });
			mapper.Component<MyCompo>(x =>
			                          {
			                          	x.ManyToOne(c => c.AManyToOne);
			                          	x.Component(c => c.NestedCompo);
			                          });
			mapper.Component<MyNestedCompo>(x =>
			                                {
			                                	x.Component(c => c.Owner);
			                                	x.Property(c => c.Something);
			                                });
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private HbmMapping GetMappingWithParentInCompo()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(x =>
			{
				x.Id(c => c.Id);
				x.Component(c => c.Compo);
				x.Bag(c => c.Compos, cm => { });
			});
			mapper.Component<MyCompo>(x =>
			{
				x.Parent(c => c.AManyToOne);
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
		public void WhenMapComoponetWithNestBidirectionalComponentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			var hbmMyNestedCompo = hbmMyCompo.Properties.OfType<HbmComponent>().Single();

			hbmMyNestedCompo.Properties.Should().Have.Count.EqualTo(1);
			hbmMyNestedCompo.Parent.Should().Not.Be.Null();
			hbmMyNestedCompo.Parent.name.Should().Be("Owner");
		}

		[Test]
		public void WhenCollectionOfComoponetsWithNestBidirectionalComponentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			var hbmMyNestedCompo = hbmMyCompo.Properties.OfType<HbmNestedCompositeElement>().Single();

			hbmMyNestedCompo.Properties.Should().Have.Count.EqualTo(1);
			hbmMyNestedCompo.Parent.Should().Not.Be.Null();
			hbmMyNestedCompo.Parent.name.Should().Be("Owner");
		}

		[Test]
		public void WhenMapComoponetWithManyToOneThenMapManyToOne()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			hbmMyCompo.Properties.OfType<HbmManyToOne>().Should().Have.Count.EqualTo(1);
			hbmMyCompo.Parent.Should().Be.Null();
		}

		[Test]
		public void WhenCollectionOfComoponetsWithManyToOneThenMapManyToOne()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmMyCompo.Properties.OfType<HbmManyToOne>().Should().Have.Count.EqualTo(1);
			hbmMyCompo.Parent.Should().Be.Null();
		}

		[Test]
		public void WhenMapComoponetWithParentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			hbmMyCompo.Properties.OfType<HbmManyToOne>().Should().Be.Empty();
			hbmMyCompo.Parent.Should().Not.Be.Null();
			hbmMyCompo.Parent.name.Should().Be("AManyToOne");
		}

		[Test]
		public void WhenCollectionOfComoponetsWithParentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			hbmMyCompo.Properties.OfType<HbmManyToOne>().Should().Be.Empty();
			hbmMyCompo.Parent.Should().Not.Be.Null();
			hbmMyCompo.Parent.name.Should().Be("AManyToOne");
		}
	}
}