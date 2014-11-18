using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(hbmMyNestedCompo.Properties.Count(), Is.EqualTo(1));
			Assert.That(hbmMyNestedCompo.Parent, Is.Not.Null);
			Assert.That(hbmMyNestedCompo.Parent.name, Is.EqualTo("Owner"));
		}

		[Test]
		public void WhenCollectionOfComoponetsWithNestBidirectionalComponentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			var hbmMyNestedCompo = hbmMyCompo.Properties.OfType<HbmNestedCompositeElement>().Single();

			Assert.That(hbmMyNestedCompo.Properties.Count(), Is.EqualTo(1));
			Assert.That(hbmMyNestedCompo.Parent, Is.Not.Null);
			Assert.That(hbmMyNestedCompo.Parent.name, Is.EqualTo("Owner"));
		}

		[Test]
		public void WhenMapComoponetWithManyToOneThenMapManyToOne()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			Assert.That(hbmMyCompo.Properties.OfType<HbmManyToOne>().Count(), Is.EqualTo(1));
			Assert.That(hbmMyCompo.Parent, Is.Null);
		}

		[Test]
		public void WhenCollectionOfComoponetsWithManyToOneThenMapManyToOne()
		{
			HbmMapping mapping = GetMappingWithManyToOneInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			Assert.That(hbmMyCompo.Properties.OfType<HbmManyToOne>().Count(), Is.EqualTo(1));
			Assert.That(hbmMyCompo.Parent, Is.Null);
		}

		[Test]
		public void WhenMapComoponetWithParentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmMyCompo = hbmClass.Properties.OfType<HbmComponent>().Single();
			Assert.That(hbmMyCompo.Properties.OfType<HbmManyToOne>(), Is.Empty);
			Assert.That(hbmMyCompo.Parent, Is.Not.Null);
			Assert.That(hbmMyCompo.Parent.name, Is.EqualTo("AManyToOne"));
		}

		[Test]
		public void WhenCollectionOfComoponetsWithParentThenMapParent()
		{
			HbmMapping mapping = GetMappingWithParentInCompo();

			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single();

			var hbmMyCompo = (HbmCompositeElement)hbmBag.ElementRelationship;
			Assert.That(hbmMyCompo.Properties.OfType<HbmManyToOne>(), Is.Empty);
			Assert.That(hbmMyCompo.Parent, Is.Not.Null);
			Assert.That(hbmMyCompo.Parent.name, Is.EqualTo("AManyToOne"));
		}
	}
}