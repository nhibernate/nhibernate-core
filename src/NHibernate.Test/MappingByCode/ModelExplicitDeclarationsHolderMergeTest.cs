using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode
{
	public class ModelExplicitDeclarationsHolderMergeTest
	{
		private class ExplicitDeclarationsHolderMock: IModelExplicitDeclarationsHolder
		{
			public ExplicitDeclarationsHolderMock()
			{
				PropertiesGettersUsed = new HashSet<string>();
			}
			private readonly HashSet<MemberInfo> any = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
			private readonly HashSet<System.Type> components = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> idBags = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> keyManyToManyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> itemManyToManyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> manyToAnyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> manyToOneRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> naturalIds = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> oneToManyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> oneToOneRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> composedIds = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> properties = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> dynamicComponents = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> persistentMembers = new HashSet<MemberInfo>();
			private readonly HashSet<System.Type> rootEntities = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
			private readonly HashSet<SplitDefinition> splitDefinitions = new HashSet<SplitDefinition>();
			private readonly HashSet<System.Type> tablePerClassEntities = new HashSet<System.Type>();
			private readonly HashSet<System.Type> tablePerClassHierarchyEntities = new HashSet<System.Type>();
			private readonly HashSet<System.Type> tablePerConcreteClassEntities = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> versionProperties = new HashSet<MemberInfo>();

			public HashSet<string> PropertiesGettersUsed { get; set; }

			#region IModelExplicitDeclarationsHolder Members

			public IEnumerable<System.Type> RootEntities
			{
				get
				{
					PropertiesGettersUsed.Add("RootEntities");
					return rootEntities;
				}
			}

			public IEnumerable<System.Type> Components
			{
				get
				{
					PropertiesGettersUsed.Add("Components");
					return components;
				}
			}

			public IEnumerable<System.Type> TablePerClassEntities
			{
				get
				{
					PropertiesGettersUsed.Add("TablePerClassEntities");
					return tablePerClassEntities;
				}
			}

			public IEnumerable<System.Type> TablePerClassHierarchyEntities
			{
				get
				{
					PropertiesGettersUsed.Add("TablePerClassHierarchyEntities");
					return tablePerClassHierarchyEntities;
				}
			}

			public IEnumerable<System.Type> TablePerConcreteClassEntities
			{
				get
				{
					PropertiesGettersUsed.Add("TablePerConcreteClassEntities");
					return tablePerConcreteClassEntities;
				}
			}

			public IEnumerable<MemberInfo> OneToOneRelations
			{
				get
				{
					PropertiesGettersUsed.Add("OneToOneRelations");
					return oneToOneRelations;
				}
			}

			public IEnumerable<MemberInfo> ManyToOneRelations
			{
				get
				{
					PropertiesGettersUsed.Add("ManyToOneRelations");
					return manyToOneRelations;
				}
			}

			public IEnumerable<MemberInfo> KeyManyToManyRelations
			{
				get
				{
					PropertiesGettersUsed.Add("KeyManyToManyRelations");
					return keyManyToManyRelations;
				}
			}

			public IEnumerable<MemberInfo> ItemManyToManyRelations
			{
				get
				{
					PropertiesGettersUsed.Add("ItemManyToManyRelations");
					return itemManyToManyRelations;
				}
			}

			public IEnumerable<MemberInfo> OneToManyRelations
			{
				get
				{
					PropertiesGettersUsed.Add("OneToManyRelations");
					return oneToManyRelations;
				}
			}

			public IEnumerable<MemberInfo> ManyToAnyRelations
			{
				get
				{
					PropertiesGettersUsed.Add("ManyToAnyRelations");
					return manyToAnyRelations;
				}
			}

			public IEnumerable<MemberInfo> Any
			{
				get
				{
					PropertiesGettersUsed.Add("Any");
					return any;
				}
			}

			public IEnumerable<MemberInfo> Poids
			{
				get
				{
					PropertiesGettersUsed.Add("Poids");
					return poids;
				}
			}

			public IEnumerable<MemberInfo> ComposedIds
			{
				get
				{
					PropertiesGettersUsed.Add("ComposedIds");
					return composedIds;
				}
			}

			public IEnumerable<MemberInfo> VersionProperties
			{
				get
				{
					PropertiesGettersUsed.Add("VersionProperties");
					return versionProperties;
				}
			}

			public IEnumerable<MemberInfo> NaturalIds
			{
				get
				{
					PropertiesGettersUsed.Add("NaturalIds");
					return naturalIds;
				}
			}

			public IEnumerable<MemberInfo> Sets
			{
				get
				{
					PropertiesGettersUsed.Add("Sets");
					return sets;
				}
			}

			public IEnumerable<MemberInfo> Bags
			{
				get
				{
					PropertiesGettersUsed.Add("Bags");
					return bags;
				}
			}

			public IEnumerable<MemberInfo> IdBags
			{
				get
				{
					PropertiesGettersUsed.Add("IdBags");
					return idBags;
				}
			}

			public IEnumerable<MemberInfo> Lists
			{
				get
				{
					PropertiesGettersUsed.Add("Lists");
					return lists;
				}
			}

			public IEnumerable<MemberInfo> Arrays
			{
				get
				{
					PropertiesGettersUsed.Add("Arrays");
					return arrays;
				}
			}

			public IEnumerable<MemberInfo> Dictionaries
			{
				get
				{
					PropertiesGettersUsed.Add("Dictionaries");
					return dictionaries;
				}
			}

			public IEnumerable<MemberInfo> Properties
			{
				get
				{
					PropertiesGettersUsed.Add("Properties");
					return properties;
				}
			}

			public IEnumerable<MemberInfo> DynamicComponents
			{
				get
				{
					PropertiesGettersUsed.Add("DynamicComponents");
					return dynamicComponents;
				}
			}

			public IEnumerable<MemberInfo> PersistentMembers
			{
				get
				{
					PropertiesGettersUsed.Add("PersistentMembers");
					return persistentMembers;
				}
			}

			public IEnumerable<SplitDefinition> SplitDefinitions
			{
				get
				{
					PropertiesGettersUsed.Add("SplitDefinitions");
					return splitDefinitions;
				}
			}

			public IEnumerable<string> GetSplitGroupsFor(System.Type type)
			{
				return Enumerable.Empty<string>();
			}

			public string GetSplitGroupFor(MemberInfo member)
			{
				return null;
			}

			public System.Type GetDynamicComponentTemplate(MemberInfo member)
			{
				return null;
			}

			public void AddAsRootEntity(System.Type type) { }
			public void AddAsComponent(System.Type type) { }
			public void AddAsTablePerClassEntity(System.Type type) { }
			public void AddAsTablePerClassHierarchyEntity(System.Type type) { }
			public void AddAsTablePerConcreteClassEntity(System.Type type) { }
			public void AddAsOneToOneRelation(MemberInfo member) { }
			public void AddAsManyToOneRelation(MemberInfo member) { }
			public void AddAsManyToManyKeyRelation(MemberInfo member) { }
			public void AddAsManyToManyItemRelation(MemberInfo member) {}
			public void AddAsOneToManyRelation(MemberInfo member) { }
			public void AddAsManyToAnyRelation(MemberInfo member) {}

			public void AddAsAny(MemberInfo member) { }
			public void AddAsPoid(MemberInfo member) { }
			public void AddAsPartOfComposedId(MemberInfo member) { }
			public void AddAsVersionProperty(MemberInfo member) { }
			public void AddAsNaturalId(MemberInfo member) { }
			public void AddAsSet(MemberInfo member) { }
			public void AddAsBag(MemberInfo member) { }
			public void AddAsIdBag(MemberInfo member) { }
			public void AddAsList(MemberInfo member) { }
			public void AddAsArray(MemberInfo member) { }
			public void AddAsMap(MemberInfo member) { }
			public void AddAsProperty(MemberInfo member) { }
			public void AddAsPersistentMember(MemberInfo member) { }
			public void AddAsPropertySplit(SplitDefinition definition) { }
			public void AddAsDynamicComponent(MemberInfo member, System.Type componentTemplate) { }

			#endregion
		}
		private readonly MemberInfo property = typeof (MyClass).GetProperty("Bar");

		[Test]
		public void WhenMergeNullsThenNotThrows()
		{
			Assert.That(() => ((ExplicitDeclarationsHolder) null).Merge(new ExplicitDeclarationsHolder()), Throws.Nothing);
			Assert.That(() => (new ExplicitDeclarationsHolder()).Merge(null), Throws.Nothing);
		}

		[Test]
		public void MergeSplitDefinitions()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPropertySplit(new SplitDefinition(typeof (MyClass), "foo", property));

			destination.Merge(source);
			Assert.That(destination.SplitDefinitions, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergePersistentMembers()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPersistentMember(property);

			destination.Merge(source);
			Assert.That(destination.PersistentMembers, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeProperties()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsProperty(property);

			destination.Merge(source);
			Assert.That(destination.Properties, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeDictionaries()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsMap(property);

			destination.Merge(source);
			Assert.That(destination.Dictionaries, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeArrays()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsArray(property);

			destination.Merge(source);
			Assert.That(destination.Arrays, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeLists()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsList(property);

			destination.Merge(source);
			Assert.That(destination.Lists, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeIdBags()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsIdBag(property);

			destination.Merge(source);
			Assert.That(destination.IdBags, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeBags()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsBag(property);

			destination.Merge(source);
			Assert.That(destination.Bags, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeSets()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsSet(property);

			destination.Merge(source);
			Assert.That(destination.Sets, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeNaturalIds()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsNaturalId(property);

			destination.Merge(source);
			Assert.That(destination.NaturalIds, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeVersionProperties()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsVersionProperty(property);

			destination.Merge(source);
			Assert.That(destination.VersionProperties, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergePoids()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPoid(property);

			destination.Merge(source);
			Assert.That(destination.Poids, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeAny()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsAny(property);

			destination.Merge(source);
			Assert.That(destination.Any, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeOneToManyRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsOneToManyRelation(property);

			destination.Merge(source);
			Assert.That(destination.OneToManyRelations, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeManyToAnyRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsManyToAnyRelation(property);

			destination.Merge(source);
			Assert.That(destination.ManyToAnyRelations, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeManyToManyRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsManyToManyItemRelation(property);

			destination.Merge(source);
			Assert.That(destination.ItemManyToManyRelations, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeManyToOneRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsManyToOneRelation(property);

			destination.Merge(source);
			Assert.That(destination.ManyToOneRelations, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeOneToOneRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsOneToOneRelation(property);

			destination.Merge(source);
			Assert.That(destination.OneToOneRelations, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeTablePerConcreteClassEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerConcreteClassEntity(typeof (MyClass));

			destination.Merge(source);
			Assert.That(destination.TablePerConcreteClassEntities, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeTablePerClassHierarchyEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerClassHierarchyEntity(typeof (MyClass));

			destination.Merge(source);
			Assert.That(destination.TablePerClassHierarchyEntities, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeTablePerClassEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerClassEntity(typeof (MyClass));

			destination.Merge(source);
			Assert.That(destination.TablePerClassEntities, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeComponents()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsComponent(typeof (MyClass));

			destination.Merge(source);
			Assert.That(destination.Components, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeRootEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsRootEntity(typeof (MyClass));

			destination.Merge(source);
			Assert.That(destination.RootEntities, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeDynamicComponents()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsDynamicComponent(property, typeof(MyClass));

			destination.Merge(source);
			Assert.That(destination.DynamicComponents, Has.Count.EqualTo(1));
			Assert.That(destination.GetDynamicComponentTemplate(property), Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void MergeComposedId()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPartOfComposedId(property);

			destination.Merge(source);
			Assert.That(destination.ComposedIds, Has.Count.EqualTo(1));
		}

		[Test]
		public void MergeShouldGetAllPropertiesOfPatternsAppliersHolderOfBothSide()
		{
			// this test is to check that, at least, we are getting all properties (to avoid to forget something)
			string[] propertiesOfIPatternsAppliersHolder =
				typeof(IModelExplicitDeclarationsHolder).GetProperties().Select(x => x.Name).ToArray();

			var first = new ExplicitDeclarationsHolderMock();
			var second = new ExplicitDeclarationsHolderMock();

			first.Merge(second);

			Assert.That(second.PropertiesGettersUsed, Is.EquivalentTo(propertiesOfIPatternsAppliersHolder));
		}

		#region Nested type: MyClass

		private class MyClass
		{
			public string Bar { get; set; }
		}

		#endregion
	}
}