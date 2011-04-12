using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class PropertyColumnNameTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string Fake1 { get; set; }
		}

		private class MyClassWithComponent
		{
			public int Id { get; set; }
			public MyComponent Component1 { get; set; }
			public IEnumerable<MyComponent> Components { get; set; }
		}

		private class MyComponent
		{
			public string Fake0 { get; set; }
		}

		[Test]
		public void WhenAtClassLevelThenNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = mapping.RootClasses[0];
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single(x => x.Name == "Fake1");
			hbmProperty.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenFirstLevelIsCollectionThenNoMatch()
		{
			var mapper = new ConventionModelMapper();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClassWithComponent) });
			var hbmClass = mapping.RootClasses[0];
			var hbmBag = hbmClass.Properties.OfType<HbmBag>().Single(x => x.Name == "Components");
			var hbmCompositeElement = (HbmCompositeElement) hbmBag.ElementRelationship;
			var hbmProperty = hbmCompositeElement.Properties.OfType<HbmProperty>().Single(x => x.Name == "Fake0");
			hbmProperty.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenAtComponentLevelThenMatch()
		{
			var mapper = new ConventionModelMapper();
			var mapping = mapper.CompileMappingFor(new[] { typeof(MyClassWithComponent) });
			var hbmClass = mapping.RootClasses[0];
			var hbmComponent = hbmClass.Properties.OfType<HbmComponent>().Single();
			var hbmProperty = hbmComponent.Properties.OfType<HbmProperty>().Single(x => x.Name == "Fake0");
			hbmProperty.Columns.Single().name.Should().Be("Component1Fake0");
		}
	}
}