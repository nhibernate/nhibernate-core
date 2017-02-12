using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(inspector.IsPersistentProperty(typeof(MyEntity).GetProperty("ReadOnly")), Is.True);
		}

		[Test]
		public void WhenReadonlyNotDeclaredThenIsNotPersistentProperty()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			Assert.That(inspector.IsPersistentProperty(typeof(MyEntity).GetProperty("ReadOnly")), Is.False);
		}

		[Test]
		public void IncludesReadOnlyWithField()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;
			
			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnlyWithField");
			Assert.That(inspector.IsPersistentProperty(pi), Is.True);
		}

		[Test]
		public void IncludesNoReadOnly()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			PropertyInfo pi = typeof(MyEntity).GetProperty("NoReadOnly");
			Assert.That(inspector.IsPersistentProperty(pi), Is.True);

			pi = typeof(MyEntity).GetProperty("WriteOnly");
			Assert.That(inspector.IsPersistentProperty(pi), Is.True);
		}

		[Test]
		public void IncludesFieldsWhenExplicitDeclared()
		{
			var autoinspector = new SimpleModelInspector();
			var mapper = new ModelMapper(autoinspector);
			mapper.Class<MyEntity>(map => map.Property("pizza", x => { }));
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetField("pizza", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(inspector.IsPersistentProperty(pi), Is.True);
		}

		[Test]
		public void DoesNotIncludesFieldsByDefault()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetField("pizza", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(inspector.IsPersistentProperty(pi), Is.False);
		}

		[Test]
		public void IncludesAutoprop()
		{
			var autoinspector = new SimpleModelInspector();
			var inspector = (IModelInspector)autoinspector;

			var pi = typeof(MyEntity).GetProperty("AutoPropWithPrivateSet");
			Assert.That(inspector.IsPersistentProperty(pi), Is.True);
		}

	}
}