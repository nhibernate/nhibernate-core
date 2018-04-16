using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	[TestFixture]
	public class ComponentWithBagOfNestedComponentsTests : TestCase
	{
		public class OwnedItem
		{
			public virtual string NameOnClient { get; set; }
		}
		class Owner
		{
			private Lazy<IList<OwnedItem>> _ownedItems = new Lazy<IList<OwnedItem>>(() => new List<OwnedItem>());

			public virtual long Id { get; set; }

			public virtual IList<OwnedItem> OwnedItems
			{
				get { return _ownedItems.Value; }
				set { _ownedItems = new Lazy<IList<OwnedItem>>(() => value); }
			}
		}

		class OwnerChildOne : Owner
		{
		}

		class OwnerChildTwo: Owner
		{
		}

		public class Intermediate
		{
			private Lazy<IList<OwnedItem>> _children = new Lazy<IList<OwnedItem>>(() => new List<OwnedItem>());
			public virtual IList<OwnedItem> Children
			{
				get { return _children.Value; }
				set { _children = new Lazy<IList<OwnedItem>>(() => value); }
			}
		}

		public class OwnerWithIntermediate
		{
			public virtual long Id { get; set; }
			public virtual Intermediate Intermediate { get; set; }
		}

		protected override IList Mappings
		{
			get
			{
				// We can perform these tests without
				// creating a data schema
				return Array.Empty<string>();
			}
		}

		[Test]
		public void BagIsPropertyOfEntity()
		{
			var mapper = new ModelMapper();
			mapper.Class<Owner>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Bag<OwnedItem>
					(
						parent => parent.OwnedItems,
						bagPropertyMapper =>
						{
							bagPropertyMapper.Table("Child");
							bagPropertyMapper.Key(k => k.Column("Parent_Id"));
						},
						r => r.Component(
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
					   )
					);
			});
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			HbmBag hbmBag = mappings
								.Items.Cast<HbmClass>()
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();

			Assert.That(hbmBag.Item, Is.InstanceOf<HbmCompositeElement>());
			HbmCompositeElement childElement = (HbmCompositeElement)hbmBag.Item;
			Assert.That(childElement.Properties.Count(), Is.EqualTo(1));
			HbmProperty propertyMapping = childElement.Properties.Cast<HbmProperty>().Single();
			Assert.That(propertyMapping.Name, Is.EqualTo("NameOnClient"));
			Assert.That(propertyMapping.Columns.Count(), Is.EqualTo(1));
			Assert.That(propertyMapping.Columns.Single().name, Is.EqualTo("NameInDatabase"));
		}

		[Test]
		public void BagIsPropertyOfComponent()
		{
			var mapper = new ModelMapper();
			mapper.Class<OwnerWithIntermediate>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Component(
						p => p.Intermediate,
						componentMapper =>
							componentMapper.Bag<OwnedItem>
							(
								intermediate => intermediate.Children,
								bagPropertyMapper =>
								{
									bagPropertyMapper.Table("Child");
									bagPropertyMapper.Key(k => k.Column("Parent_Id"));
								},
								r => r.Component(child =>
													child.Property
													(
														c => c.NameOnClient,
														pm =>
														{
															pm.Column("NameInDatabase");
														}
													)
												)
							)
					);
			});

			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			HbmBag hbmBag = mappings
								.Items.Cast<HbmClass>()
								.Single()
								.Properties.Cast<HbmComponent>()
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();

			Assert.That(hbmBag.Item, Is.InstanceOf<HbmCompositeElement>());
			HbmCompositeElement childElement = (HbmCompositeElement)hbmBag.Item;
			Assert.That(childElement.Properties.Count(), Is.EqualTo(1));
			HbmProperty propertyMapping = childElement.Properties.Cast<HbmProperty>().Single();
			Assert.That(propertyMapping.Name, Is.EqualTo("NameOnClient"));
			Assert.That(propertyMapping.Columns.Count(), Is.EqualTo(1));
			Assert.That(propertyMapping.Columns.Single().name, Is.EqualTo("NameInDatabase"));
		}

		[Test]
		public void PropertyCustomizerDifferentiatesBetweenChildClasses()
		{
			var mapper = new ModelMapper();
			mapper.Class<OwnerChildOne>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Bag<OwnedItem>
					(
						parent => parent.OwnedItems,
						bagPropertyMapper =>
						{
							bagPropertyMapper.Table("ChildOne");
							bagPropertyMapper.Key(k => k.Column("Parent_Id"));
						},
						r => r.Component(
							child =>
							{
								child.Property
								(
									c => c.NameOnClient,
									pm =>
									{
										pm.Column("OwnerChildOne_CustomColumnName");
									}
								);
							}
					   )
					);
			});
			mapper.Class<OwnerChildTwo>(classMapper =>
			{
				classMapper.Id(p => p.Id);
				classMapper.Bag<OwnedItem>
					(
						parent => parent.OwnedItems,
						bagPropertyMapper =>
						{
							bagPropertyMapper.Table("ChildTwo");
							bagPropertyMapper.Key(k => k.Column("Parent_Id"));
						},
						r => r.Component(
							child =>
							{
								child.Property
								(
									c => c.NameOnClient,
									pm =>
									{
										pm.Column("OwnerChildTwo_CustomColumnName");
									}
								);
							}
					   )
					);
			});

			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();


			HbmBag bag1 = mappings
								.Items.Cast<HbmClass>()
								.Where(c => c.Name == typeof(OwnerChildOne).FullName)
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();

			HbmCompositeElement childElement1 = (HbmCompositeElement)bag1.Item;
			HbmProperty propertyMapping1 = childElement1.Properties.Cast<HbmProperty>().Single();
			Assert.That(propertyMapping1.Columns.Single().name, Is.EqualTo("OwnerChildOne_CustomColumnName"));

			HbmBag bag2 = mappings
								.Items.Cast<HbmClass>()
								.Where(c => c.Name == typeof(OwnerChildTwo).FullName)
								.Single()
								.Properties.Cast<HbmBag>()
								.Single();

			HbmCompositeElement childElement2 = (HbmCompositeElement)bag2.Item;
			HbmProperty propertyMapping2 = childElement2.Properties.Cast<HbmProperty>().Single();
			Assert.That(propertyMapping2.Columns.Single().name, Is.EqualTo("OwnerChildTwo_CustomColumnName"));
		}


	}
}
