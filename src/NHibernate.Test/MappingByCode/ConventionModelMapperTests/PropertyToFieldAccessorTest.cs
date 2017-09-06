using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	[TestFixture]
	public class PropertyToFieldAccessorTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			// Used by reflection
#pragma warning disable CS0169 // The field is never used
			private string aField;
#pragma warning restore CS0169 // The field is never used
			public int AProp { get; set; }

			private ICollection<int> withDifferentBackField;
			public IEnumerable<int> WithDifferentBackField
			{
				get { return withDifferentBackField; }
				set { withDifferentBackField = value as ICollection<int>; }
			}

			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private string readOnlyWithSameBackField;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
			public string ReadOnlyWithSameBackField
			{
				get { return readOnlyWithSameBackField; }
			}

			public string PropertyWithoutField
			{
				get { return ""; }
			}

			private string sameTypeOfBackField;
			public string SameTypeOfBackField
			{
				get { return sameTypeOfBackField; }
				set { sameTypeOfBackField = value; }
			}

			private int setOnlyProperty;
			public int SetOnlyProperty
			{
				set { setOnlyProperty = value; }
			}
		}

		[Test]
		public void WhenFieldAccessToField()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(mc => mc.Property("aField", x => { }));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "aField");
			Assert.That(hbmProperty.Access, Is.EqualTo("field"));
		}

		[Test]
		public void WhenAutoPropertyNoAccessor()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "AProp");
			Assert.That(hbmProperty.Access, Is.Null.Or.Empty);
		}

		[Test]
		public void WhenPropertyWithSameBackFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "SameTypeOfBackField");
			Assert.That(hbmProperty.Access, Is.Null.Or.Empty);
		}

		[Test]
		public void WhenReadOnlyPropertyWithSameBackFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "ReadOnlyWithSameBackField");
			Assert.That(hbmProperty.Access, Does.Not.Contain("field"));
		}

		[Test]
		public void WhenPropertyWithoutFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(mc => mc.Property(x => x.PropertyWithoutField));

			var hbmMapping = mapper.CompileMappingFor(new[] {typeof (MyClass)});

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "PropertyWithoutField");
			Assert.That(hbmProperty.Access, Does.Not.Contain("field"));
		}

		[Test]
		public void WhenPropertyWithDifferentBackFieldMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "WithDifferentBackField");
			Assert.That(hbmProperty.Access, Does.Contain("field"));
		}

		[Test]
		public void WhenSetOnlyPropertyNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "SetOnlyProperty");
			Assert.That(hbmProperty.Access, Is.Null.Or.Not.Contain("field"));
		}
	}
}
