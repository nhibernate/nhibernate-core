using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode
{
	public class CustomizerHolderMergeTest
	{
		private class MyClass
		{
			public string Bar { get; set; }
		}

		private PropertyPath propertyPath = new PropertyPath(null, typeof(MyClass).GetProperty("Bar"));

		[Test]
		public void WhenMergeWithNullThenNotThrow()
		{
			var emptyHolder = new CustomizersHolder();
			Assert.That(() => emptyHolder.Merge(null), Throws.Nothing);
		}

		[Test]
		public void MergeShouldMergeAnyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IAnyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IAnyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeBagPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IBagPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IBagPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeIdBagPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IIdBagPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IIdBagPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeCollectionPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (ICollectionPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IBagPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeElementMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IElementMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IElementMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeManyToManyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IManyToManyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IManyToManyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeManyToAnyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IManyToAnyMapper) null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeOneToManyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IOneToManyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IOneToManyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeComponentAttributesMapperOnProperty()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IComponentAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IComponentAttributesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeListPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IListPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IListPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeManyToOneMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IManyToOneMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IManyToOneMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeMapPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IMapPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IMapPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeMapKeyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IMapKeyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IMapKeyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeMapKeyManyToManyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IMapKeyManyToManyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeOneToOneMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IOneToOneMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IOneToOneMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergePropertyMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IPropertyMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IPropertyMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeSetPropertiesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (ISetPropertiesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (ISetPropertiesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeJoinedSubclassAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(typeof(MyClass), (IJoinedSubclassAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (IJoinedSubclassAttributesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeClassMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(typeof(MyClass), (IClassMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (IClassMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeSubclassMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(typeof(MyClass), (ISubclassMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (ISubclassMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeJoinAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;
			
			holder.AddCustomizer(typeof(MyClass), (IJoinAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (IJoinAttributesMapper)null);
			
			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeUnionSubclassAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(typeof(MyClass), (IUnionSubclassAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (IUnionSubclassAttributesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeComponentAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(typeof(MyClass), (IComponentAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(typeof(MyClass), (IComponentAttributesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeDynamicComponentAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IDynamicComponentAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IDynamicComponentAttributesMapper)null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void MergeShouldMergeComponentAsIdAttributesMapper()
		{
			var emptyHolder = new CustomizersHolder();
			var holder = new CustomizersHolder();
			var called = false;

			holder.AddCustomizer(propertyPath, (IComponentAsIdAttributesMapper x) => called = true);
			emptyHolder.Merge(holder);
			emptyHolder.InvokeCustomizers(propertyPath, (IComponentAsIdAttributesMapper)null);

			Assert.That(called, Is.True);
		}
	}
}