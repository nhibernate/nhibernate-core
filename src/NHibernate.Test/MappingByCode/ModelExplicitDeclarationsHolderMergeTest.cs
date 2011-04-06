using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode
{
	public class ModelExplicitDeclarationsHolderMergeTest
	{
		private readonly MemberInfo property = typeof (MyClass).GetProperty("Bar");

		[Test]
		public void WhenMergeNullsThenNotThrows()
		{
			Executing.This(() => ((EmptyHolder) null).Merge(new EmptyHolder())).Should().NotThrow();
			Executing.This(() => (new EmptyHolder()).Merge(null)).Should().NotThrow();
		}

		[Test]
		public void MergeSplitDefinitions()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsPropertySplit(typeof (MyClass), "foo", property);

			destination.Merge(source);
			destination.SplitDefinitions.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeProperties()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsProperty(property);

			destination.Merge(source);
			destination.Properties.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeDictionaries()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsMap(property);

			destination.Merge(source);
			destination.Dictionaries.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeArrays()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsArray(property);

			destination.Merge(source);
			destination.Arrays.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeLists()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsList(property);

			destination.Merge(source);
			destination.Lists.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeIdBags()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsIdBag(property);

			destination.Merge(source);
			destination.IdBags.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeBags()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsBag(property);

			destination.Merge(source);
			destination.Bags.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeSets()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsSet(property);

			destination.Merge(source);
			destination.Sets.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeNaturalIds()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsNaturalId(property);

			destination.Merge(source);
			destination.NaturalIds.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeVersionProperties()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsVersionProperty(property);

			destination.Merge(source);
			destination.VersionProperties.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergePoids()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsPoid(property);

			destination.Merge(source);
			destination.Poids.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeAny()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsAny(property);

			destination.Merge(source);
			destination.Any.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeOneToManyRelations()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsOneToManyRelation(property);

			destination.Merge(source);
			destination.OneToManyRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeManyToManyRelations()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsManyToManyRelation(property);

			destination.Merge(source);
			destination.ManyToManyRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeManyToOneRelations()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsManyToOneRelation(property);

			destination.Merge(source);
			destination.ManyToOneRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeOneToOneRelations()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsOneToOneRelation(property);

			destination.Merge(source);
			destination.OneToOneRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerConcreteClassEntities()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsTablePerConcreteClassEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerConcreteClassEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerClassHierarchyEntities()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsTablePerClassHierarchyEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerClassHierarchyEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerClassEntities()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsTablePerClassEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerClassEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeComponents()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsComponent(typeof (MyClass));

			destination.Merge(source);
			destination.Components.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeRootEntities()
		{
			var destination = new EmptyHolder();
			var source = new EmptyHolder();
			source.AddAsRootEntity(typeof (MyClass));

			destination.Merge(source);
			destination.RootEntities.Should().Have.Count.EqualTo(1);
		}

		#region Nested type: EmptyHolder

		public class EmptyHolder : IModelExplicitDeclarationsHolder
		{
			private readonly HashSet<MemberInfo> any = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> arrays = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> bags = new HashSet<MemberInfo>();
			private readonly HashSet<System.Type> components = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> dictionaries = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> idBags = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> lists = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> manyToManyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> manyToOneRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> naturalIds = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> oneToManyRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> oneToOneRelations = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> poids = new HashSet<MemberInfo>();
			private readonly HashSet<MemberInfo> properties = new HashSet<MemberInfo>();
			private readonly HashSet<System.Type> rootEntities = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> sets = new HashSet<MemberInfo>();
			private readonly HashSet<SplitDefinition> splitDefinitions = new HashSet<SplitDefinition>();
			private readonly HashSet<System.Type> tablePerClassEntities = new HashSet<System.Type>();
			private readonly HashSet<System.Type> tablePerClassHierarchyEntities = new HashSet<System.Type>();
			private readonly HashSet<System.Type> tablePerConcreteClassEntities = new HashSet<System.Type>();
			private readonly HashSet<MemberInfo> versionProperties = new HashSet<MemberInfo>();

			#region IModelExplicitDeclarationsHolder Members

			public IEnumerable<System.Type> RootEntities
			{
				get { return rootEntities; }
			}

			public IEnumerable<System.Type> Components
			{
				get { return components; }
			}

			public IEnumerable<System.Type> TablePerClassEntities
			{
				get { return tablePerClassEntities; }
			}

			public IEnumerable<System.Type> TablePerClassHierarchyEntities
			{
				get { return tablePerClassHierarchyEntities; }
			}

			public IEnumerable<System.Type> TablePerConcreteClassEntities
			{
				get { return tablePerConcreteClassEntities; }
			}

			public IEnumerable<MemberInfo> OneToOneRelations
			{
				get { return oneToOneRelations; }
			}

			public IEnumerable<MemberInfo> ManyToOneRelations
			{
				get { return manyToOneRelations; }
			}

			public IEnumerable<MemberInfo> ManyToManyRelations
			{
				get { return manyToManyRelations; }
			}

			public IEnumerable<MemberInfo> OneToManyRelations
			{
				get { return oneToManyRelations; }
			}

			public IEnumerable<MemberInfo> Any
			{
				get { return any; }
			}

			public IEnumerable<MemberInfo> Poids
			{
				get { return poids; }
			}

			public IEnumerable<MemberInfo> VersionProperties
			{
				get { return versionProperties; }
			}

			public IEnumerable<MemberInfo> NaturalIds
			{
				get { return naturalIds; }
			}

			public IEnumerable<MemberInfo> Sets
			{
				get { return sets; }
			}

			public IEnumerable<MemberInfo> Bags
			{
				get { return bags; }
			}

			public IEnumerable<MemberInfo> IdBags
			{
				get { return idBags; }
			}

			public IEnumerable<MemberInfo> Lists
			{
				get { return lists; }
			}

			public IEnumerable<MemberInfo> Arrays
			{
				get { return arrays; }
			}

			public IEnumerable<MemberInfo> Dictionaries
			{
				get { return dictionaries; }
			}

			public IEnumerable<MemberInfo> Properties
			{
				get { return properties; }
			}

			public IEnumerable<SplitDefinition> SplitDefinitions
			{
				get { return splitDefinitions; }
			}

			public IEnumerable<string> GetSplitGroupsFor(System.Type type)
			{
				return Enumerable.Empty<string>();
			}

			public string GetSplitGroupFor(MemberInfo member)
			{
				return null;
			}

			public void AddAsRootEntity(System.Type type)
			{
				rootEntities.Add(type);
			}

			public void AddAsComponent(System.Type type)
			{
				components.Add(type);
			}

			public void AddAsTablePerClassEntity(System.Type type)
			{
				tablePerClassEntities.Add(type);
			}

			public void AddAsTablePerClassHierarchyEntity(System.Type type)
			{
				tablePerClassHierarchyEntities.Add(type);
			}

			public void AddAsTablePerConcreteClassEntity(System.Type type)
			{
				tablePerConcreteClassEntities.Add(type);
			}

			public void AddAsOneToOneRelation(MemberInfo member)
			{
				oneToOneRelations.Add(member);
			}

			public void AddAsManyToOneRelation(MemberInfo member)
			{
				manyToOneRelations.Add(member);
			}

			public void AddAsManyToManyRelation(MemberInfo member)
			{
				manyToManyRelations.Add(member);
			}

			public void AddAsOneToManyRelation(MemberInfo member)
			{
				oneToManyRelations.Add(member);
			}

			public void AddAsAny(MemberInfo member)
			{
				any.Add(member);
			}

			public void AddAsPoid(MemberInfo member)
			{
				poids.Add(member);
			}

			public void AddAsVersionProperty(MemberInfo member)
			{
				versionProperties.Add(member);
			}

			public void AddAsNaturalId(MemberInfo member)
			{
				naturalIds.Add(member);
			}

			public void AddAsSet(MemberInfo member)
			{
				sets.Add(member);
			}

			public void AddAsBag(MemberInfo member)
			{
				bags.Add(member);
			}

			public void AddAsIdBag(MemberInfo member)
			{
				idBags.Add(member);
			}

			public void AddAsList(MemberInfo member)
			{
				lists.Add(member);
			}

			public void AddAsArray(MemberInfo member)
			{
				arrays.Add(member);
			}

			public void AddAsMap(MemberInfo member)
			{
				dictionaries.Add(member);
			}

			public void AddAsProperty(MemberInfo member)
			{
				properties.Add(member);
			}

			public void AddAsPropertySplit(System.Type propertyContainer, string splitGroupId, MemberInfo member)
			{
				splitDefinitions.Add(new SplitDefinition(propertyContainer, splitGroupId, member));
			}

			#endregion
		}

		#endregion

		#region Nested type: MyClass

		private class MyClass
		{
			public string Bar { get; set; }
		}

		#endregion
	}
}