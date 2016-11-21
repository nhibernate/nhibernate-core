using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using System.Text;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class ComponentWithBagOfNestedComponentsTests : TestCase
	{
		public class Child
		{
			public virtual string NameOnClient { get; set; }
		}
		class Parent
		{
			private Lazy<IList<Child>> _children = new Lazy<IList<Child>>(() => new List<Child>());

			public virtual long Id { get; set; }

			public virtual IList<Child> Children
			{
				get { return _children.Value; }
				set { _children = new Lazy<IList<Child>>(() => value); }
			}
		}

		public class Intermediate
		{
			private Lazy<IList<Child>> _children = new Lazy<IList<Child>>(() => new List<Child>());
			public virtual IList<Child> Children
			{
				get { return _children.Value; }
				set { _children = new Lazy<IList<Child>>(() => value); }
			}
		}

		public class ParentWithIntermediate
		{
			public virtual long Id { get; set; }
			public virtual Intermediate Intermediate { get; set; }
		}

		protected override IList Mappings
		{
			get
			{
				// I may have to create a mapping file here
				// so I can create the data schema
				return new string[0];
			}
		}


		private static void MapChildTableRelationships<ParentType>(IBagPropertiesMapper<ParentType, Child> bagPropertyMapper)
		{
			bagPropertyMapper.Table("Child");
			bagPropertyMapper.Key(k => k.Column("Parent_Id"));
		}

		private static void MapChildCollectionElementRelationship(ICollectionElementRelation<Child> r)
		{
			r.Component(
							child =>
							{
								child.Property
								(
									c => c.NameOnClient,
									pm =>
									{
										pm.Column("NameInDatabase");
									}
								);
							}
					   );
		}


		private void VerifyBagMapping(HbmBag hbmBag)
		{
			Assert.That(hbmBag.Item, Is.InstanceOf<HbmCompositeElement>());
			HbmCompositeElement childElement = (HbmCompositeElement)hbmBag.Item;
			Assert.That(childElement.Properties.Count(), Is.EqualTo(1));
			HbmProperty propertyMapping = childElement.Properties.Cast<HbmProperty>().Single();
			Assert.That(propertyMapping.Name, Is.EqualTo("NameOnClient"));
			Assert.That(propertyMapping.Columns.Count(), Is.EqualTo(1));
			Assert.That(propertyMapping.Columns.Single().name, Is.EqualTo("NameInDatabase"));
		}

		private HbmMapping GetMappingWithSimpleClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Bag<Child>
					(
						parent => parent.Children,
						bagPropertyMapper => MapChildTableRelationships(bagPropertyMapper),
						r => MapChildCollectionElementRelationship(r)
					);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private HbmMapping GetMappingWithIntermediateClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<ParentWithIntermediate>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Component(
						p => p.Intermediate,
						componentMapper =>
						{
							componentMapper.Bag<Child>
							(
								intermediate => intermediate.Children,
								bagPropertyMapper => MapChildTableRelationships(bagPropertyMapper),
								r => MapChildCollectionElementRelationship(r)
							);
						}
					);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void BagIsPropertyOfEntity()
		{
			var mappings = GetMappingWithSimpleClass();
			HbmBag hbmBag = mappings
								.Items.Cast<HbmClass>()
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();
			VerifyBagMapping(hbmBag);
		}

		[Test]
		public void BagIsPropertyOfComponent()
		{
			var mappings = GetMappingWithIntermediateClass();

			HbmBag hbmBag = mappings
								.Items.Cast<HbmClass>()
								.Single()
								.Properties.Cast<HbmComponent>()
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();
			VerifyBagMapping(hbmBag);
		}

	}
}
