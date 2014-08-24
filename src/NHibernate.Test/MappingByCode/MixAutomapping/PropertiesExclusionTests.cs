using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class PropertiesExclusionTests
	{
		public class MyEntity
		{
			private string noReadOnlyWithField = null;
#pragma warning disable 169
			private string pizza;
#pragma warning restore 169

			public string ReadOnly
			{
				get { return ""; }
			}

			public string NoReadOnlyWithField
			{
				get { return noReadOnlyWithField; }
			}

			public string NoReadOnly
			{
				get { return ""; }
				set { }
			}

			public string WriteOnly
			{
				set { }
			}

			public string AutoPropWithPrivateSet { get; private set; }
		}

		[Test]
		public void WhenReadonlyDeclaredThenIsPersistentProperty()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyEntity>(map => map.Property(x => x.ReadOnly));

			var inspector = (IModelInspector)autoinspector;
			inspector.IsPersistentProperty(typeof(MyEntity).GetProperty("ReadOnly")).Should().Be.True();
		}

		[Test]
		public void WhenReadonlyNotDeclaredThenIsNotPersistentProperty()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			inspector.IsPersistentProperty(typeof(MyEntity).GetProperty("ReadOnly")).Should().Be.False();
		}

		[Test]
		public void IncludesReadOnlyWithField()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			
			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnlyWithField");
			inspector.IsPersistentProperty(pi).Should().Be.True();
		}

		[Test]
		public void IncludesNoReadOnly()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnly");
			inspector.IsPersistentProperty(pi).Should().Be.True();

			pi = typeof(MyEntity).GetProperty("WriteOnly");
			inspector.IsPersistentProperty(pi).Should().Be.True();
		}

		[Test]
		public void IncludesFieldsWhenExplicitDeclared()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyEntity>(map => map.Property("pizza", x => { }));
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetField("pizza", BindingFlags.Instance | BindingFlags.NonPublic);
			inspector.IsPersistentProperty(pi).Should().Be.True();
		}

		[Test]
		public void DoesNotIncludesFieldsByDefault()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetField("pizza", BindingFlags.Instance | BindingFlags.NonPublic);
			inspector.IsPersistentProperty(pi).Should().Be.False();
		}

		[Test]
		public void IncludesAutoprop()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetProperty("AutoPropWithPrivateSet");
			inspector.IsPersistentProperty(pi).Should().Be.True();
		}

	}
}