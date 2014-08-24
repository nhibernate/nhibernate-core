using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class PropertyToFieldAccessorTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			private string aField;
			public int AProp { get; set; }

			private ICollection<int> withDifferentBackField;
			public IEnumerable<int> WithDifferentBackField
			{
				get { return withDifferentBackField; }
				set { withDifferentBackField = value as ICollection<int>; }
			}

			private string readOnlyWithSameBackField;
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
			hbmProperty.Access.Should().Be("field");
		}

		[Test]
		public void WhenAutoPropertyNoAccessor()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "AProp");
			hbmProperty.Access.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenPropertyWithSameBackFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "SameTypeOfBackField");
			hbmProperty.Access.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenReadOnlyPropertyWithSameBackFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "ReadOnlyWithSameBackField");
			hbmProperty.Access.Should().Not.Contain("field");
		}

		[Test]
		public void WhenPropertyWithoutFieldNoMatch()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(mc => mc.Property(x => x.PropertyWithoutField));

			var hbmMapping = mapper.CompileMappingFor(new[] {typeof (MyClass)});

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "PropertyWithoutField");
			hbmProperty.Access.Should().Not.Contain("field");
		}

		[Test]
		public void WhenPropertyWithDifferentBackFieldMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "WithDifferentBackField");
			hbmProperty.Access.Should().Contain("field");
		}

		[Test]
		public void WhenSetOnlyPropertyNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.Single(x => x.Name == "SetOnlyProperty");
			hbmProperty.Access.Should().Not.Contain("field");
		}
	}
}