using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class RootClassPropertiesSplitsTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string Something0 { get; set; }
			public string SomethingA1 { get; set; }
			public string SomethingA2 { get; set; }
			public string SomethingB1 { get; set; }
			public string SomethingB2 { get; set; }
		}

		[Test, Ignore("Not implemented yet")]
		public void WhenSplittedPropertiesThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map =>
														{
															map.Id(x => x.Id, idmap => { });
															map.Join(mj=>
															{
																mj.Table("MyClassSplit1");
																mj.Property(x => x.SomethingA1);
																mj.Property(x => x.SomethingA2);
															});
															map.Join(mj =>
															{
																mj.Table("MyClassSplit2");
																mj.Property(x => x.SomethingB1);
																mj.Property(x => x.SomethingB2);
															});
															map.Property(x => x.Something0);
														});

			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<MyClass>.Property(x => x.Something0)).Should().Be.False();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<MyClass>.Property(x => x.Something0)).Should().Be.False();

			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<MyClass>.Property(x => x.SomethingA1)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit1", For<MyClass>.Property(x => x.SomethingA2)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<MyClass>.Property(x => x.SomethingB1)).Should().Be.True();
			inspector.IsTablePerClassSplit(typeof(MyClass), "MyClassSplit2", For<MyClass>.Property(x => x.SomethingB2)).Should().Be.True();
		}
	}
}