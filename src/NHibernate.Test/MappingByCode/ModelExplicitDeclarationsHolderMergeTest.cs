using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
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
			Executing.This(() => ((ExplicitDeclarationsHolder) null).Merge(new ExplicitDeclarationsHolder())).Should().NotThrow();
			Executing.This(() => (new ExplicitDeclarationsHolder()).Merge(null)).Should().NotThrow();
		}

		[Test]
		public void MergeSplitDefinitions()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPropertySplit(typeof (MyClass), "foo", property);

			destination.Merge(source);
			destination.SplitDefinitions.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeProperties()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsProperty(property);

			destination.Merge(source);
			destination.Properties.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeDictionaries()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsMap(property);

			destination.Merge(source);
			destination.Dictionaries.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeArrays()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsArray(property);

			destination.Merge(source);
			destination.Arrays.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeLists()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsList(property);

			destination.Merge(source);
			destination.Lists.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeIdBags()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsIdBag(property);

			destination.Merge(source);
			destination.IdBags.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeBags()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsBag(property);

			destination.Merge(source);
			destination.Bags.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeSets()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsSet(property);

			destination.Merge(source);
			destination.Sets.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeNaturalIds()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsNaturalId(property);

			destination.Merge(source);
			destination.NaturalIds.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeVersionProperties()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsVersionProperty(property);

			destination.Merge(source);
			destination.VersionProperties.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergePoids()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsPoid(property);

			destination.Merge(source);
			destination.Poids.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeAny()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsAny(property);

			destination.Merge(source);
			destination.Any.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeOneToManyRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsOneToManyRelation(property);

			destination.Merge(source);
			destination.OneToManyRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeManyToManyRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsManyToManyRelation(property);

			destination.Merge(source);
			destination.ManyToManyRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeManyToOneRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsManyToOneRelation(property);

			destination.Merge(source);
			destination.ManyToOneRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeOneToOneRelations()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsOneToOneRelation(property);

			destination.Merge(source);
			destination.OneToOneRelations.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerConcreteClassEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerConcreteClassEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerConcreteClassEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerClassHierarchyEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerClassHierarchyEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerClassHierarchyEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeTablePerClassEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsTablePerClassEntity(typeof (MyClass));

			destination.Merge(source);
			destination.TablePerClassEntities.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeComponents()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsComponent(typeof (MyClass));

			destination.Merge(source);
			destination.Components.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void MergeRootEntities()
		{
			var destination = new ExplicitDeclarationsHolder();
			var source = new ExplicitDeclarationsHolder();
			source.AddAsRootEntity(typeof (MyClass));

			destination.Merge(source);
			destination.RootEntities.Should().Have.Count.EqualTo(1);
		}

		#region Nested type: MyClass

		private class MyClass
		{
			public string Bar { get; set; }
		}

		#endregion
	}
}