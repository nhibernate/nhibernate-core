using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class SubclassPropertiesSplitsTests
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		private class Inherited : MyClass
		{
			public string Something0 { get; set; }
			public string SomethingA1 { get; set; }
			public string SomethingA2 { get; set; }
			public string SomethingB1 { get; set; }
			public string SomethingB2 { get; set; }
		}

		[Test]
		public void WhenSplittedPropertiesThenRegisterSplitGroupIds()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Subclass<Inherited>(map =>
			{
				map.Join("MyClassSplit1", mj =>
				{
					mj.Property(x => x.SomethingA1);
					mj.Property(x => x.SomethingA2);
				});
				map.Join("MyClassSplit2", mj =>
				{
					mj.Property(x => x.SomethingB1);
					mj.Property(x => x.SomethingB2);
				});
				map.Property(x => x.Something0);
			});

			IEnumerable<string> tablePerClassSplits = inspector.GetPropertiesSplits(typeof(Inherited));
			tablePerClassSplits.Should().Have.SameValuesAs("MyClassSplit1", "MyClassSplit2");
		}

		[Test]
		public void WhenSplittedPropertiesThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Subclass<Inherited>(map =>
			{
				map.Join("MyClassSplit1", mj =>
				{
					mj.Property(x => x.SomethingA1);
					mj.Property(x => x.SomethingA2);
				});
				map.Join("MyClassSplit2", mj =>
				{
					mj.Property(x => x.SomethingB1);
					mj.Property(x => x.SomethingB2);
				});
				map.Property(x => x.Something0);
			});

			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<Inherited>.Property(x => x.Something0)).Should().Be.False();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<Inherited>.Property(x => x.Something0)).Should().Be.False();

			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<Inherited>.Property(x => x.SomethingA1)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<Inherited>.Property(x => x.SomethingA2)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<Inherited>.Property(x => x.SomethingB1)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<Inherited>.Property(x => x.SomethingB2)).Should().Be.True();
		}

		[Test]
		public void WhenMapSplittedPropertiesThenEachPropertyIsInItsSplitGroup()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map => map.Id(x => x.Id, idmap => { }));
			mapper.Subclass<Inherited>(map =>
			{
				map.Join("MyClassSplit1", mj =>
				{
					mj.Property(x => x.SomethingA1);
					mj.Property(x => x.SomethingA2);
				});
				map.Join("MyClassSplit2", mj =>
				{
					mj.Property(x => x.SomethingB1);
					mj.Property(x => x.SomethingB2);
				});
				map.Property(x => x.Something0);
			});
			var hbmDoc = mapper.CompileMappingFor(new[] { typeof(Inherited) });

			var hbmClass = hbmDoc.SubClasses[0];
			hbmClass.Joins.Select(j => j.table).Should().Have.SameValuesAs("MyClassSplit1", "MyClassSplit2");
			hbmClass.Properties.Single().Name.Should().Be("Something0");
			var hbmSplit1 = hbmClass.Joins.Single(j => "MyClassSplit1" == j.table);
			hbmSplit1.Properties.Select(p => p.Name).Should().Have.SameValuesAs("SomethingA1", "SomethingA2");
			var hbmSplit2 = hbmClass.Joins.Single(j => "MyClassSplit2" == j.table);
			hbmSplit2.Properties.Select(p => p.Name).Should().Have.SameValuesAs("SomethingB1", "SomethingB2");
		}
	}
}