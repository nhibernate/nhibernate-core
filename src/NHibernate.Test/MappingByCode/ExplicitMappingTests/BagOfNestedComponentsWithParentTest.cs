using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class BagOfNestedComponentsWithParentTest
	{
		private class Person
		{
			public int Id { get; set; }
			public ICollection<Address> Addresses { get; set; }
		}

		private class Address
		{
			public Person Owner { get; set; }
			public string Street { get; set; }
			public Number Number { get; set; }
		}

		private class Number
		{
			public Address OwnerAddress { get; set; }
			public int Block { get; set; }
		}

		[Test]
		public void WhenMapClasByClassThenAutodiscoverParent()
		{
			var mapper = new ModelMapper();
			mapper.Component<Address>(cm =>
			{
				cm.ManyToOne(x => x.Owner);
				cm.Property(x => x.Street);
				cm.Component(x => x.Number, y => { });
			});
			mapper.Component<Number>(cm =>
			{
				cm.Component(x => x.OwnerAddress, map => { });
				cm.Property(x => x.Block);
			});
			mapper.Class<Person>(cm =>
			{
				cm.Id(x => x.Id);
				cm.Bag(x => x.Addresses, cp => { }, cr => { });
			});
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			VerifyMapping(mapping, false, "Street", "Number", "Owner");
		}

		[Test]
		public void WhenMapClassWithWrongElementsThenAutodiscoverParent()
		{
			// In this case the user will use wrong mapping-elements as ManyToOne and Component (he should realize that it end in an infinite loop)
			var mapper = new ModelMapper();
			mapper.Class<Person>(cm =>
			{
				cm.Id(x => x.Id);
				cm.Bag(x => x.Addresses, cp => { }, cr => cr.Component(ce =>
				{
					ce.ManyToOne(x => x.Owner);
					ce.Property(x => x.Street);
					ce.Component(x => x.Number, y =>
					{
						y.Component(x => x.OwnerAddress, map => { });
						y.Property(x => x.Block);
					});
				}));
			});
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			VerifyMapping(mapping, false, "Street", "Number", "Owner");
		}

		[Test]
		public void WhenMapClassElementsThenMapParent()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(cm =>
			{
				cm.Id(x => x.Id);
				cm.Bag(x => x.Addresses, cp => { }, cr => cr.Component(ce =>
				{
					ce.Parent(x => x.Owner);
					ce.Property(x => x.Street);
					ce.Component(x => x.Number, y =>
					{
						y.Parent(x => x.OwnerAddress, map => { });
						y.Property(x => x.Block);
					});
				}));
			});
			HbmMapping mapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			VerifyMapping(mapping, true, "Street", "Number");
		}

		private void VerifyMapping(HbmMapping mapping, bool hasParent, params string[] properties)
		{
			HbmClass rc = mapping.RootClasses.First(r => r.Name.Contains("Person"));
			var relation = rc.Properties.First(p => p.Name == "Addresses");
			var collection = (HbmBag)relation;
			Assert.That(collection.ElementRelationship, Is.TypeOf<HbmCompositeElement>());
			var elementRelation = (HbmCompositeElement)collection.ElementRelationship;
			Assert.That(elementRelation.Class, Is.StringContaining("Address"));
			
			// This test was modified because when the "owner" is an entity it can be mapped as many-to-one or as parent and without an explicit
			// definition of the property representing the bidiretional-relation we can't know is the mapping element (many-to-one or parent)
			Assert.That(elementRelation.Properties.Count(), Is.EqualTo(properties.Length));
			Assert.That(elementRelation.Properties.Select(p => p.Name), Is.EquivalentTo(properties));
			if (hasParent)
			{
				Assert.That(elementRelation.Parent, Is.Not.Null);
				Assert.That(elementRelation.Parent.name, Is.EqualTo("Owner"));
			}
			else
			{
				Assert.That(elementRelation.Parent, Is.Null);
			}
			
			// Nested
			var propertyNestedRelation = elementRelation.Properties.FirstOrDefault(p => p.Name == "Number");
			Assert.That(propertyNestedRelation, Is.Not.Null.And.TypeOf<HbmNestedCompositeElement>());
			var nestedRelation = (HbmNestedCompositeElement) propertyNestedRelation;
			Assert.That(nestedRelation.Class, Is.StringContaining("Number"));
			Assert.That(nestedRelation.Properties.Count(), Is.EqualTo(1));
			Assert.That(nestedRelation.Parent, Is.Not.Null);
			Assert.That(nestedRelation.Parent.name, Is.EqualTo("OwnerAddress"));
		}
	}
}