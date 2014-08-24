using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class NaturalIdTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public Related Related { get; set; }
			public MyComponent MyComponent { get; set; }
			public object Any { get; set; }
		}

		private class Related
		{
			public int Id { get; set; }
		}

		private class MyComponent
		{
			public string FirstName { get; set; }
		}

		[Test]
		public void WhenDefineNaturalIdThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.NaturalId(nidm =>
											{
												nidm.Property(x => x.Name);
												nidm.ManyToOne(x => x.Related);
												nidm.Component(x => x.MyComponent, cmap =>
												                                   {
																														 cmap.Property(y => y.FirstName);
												                                   });
												nidm.Any(x => x.Any, typeof(int), anymap => { });
											});
			});

			inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Name)).Should().Be.True();
			inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Related)).Should().Be.True();
			inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.MyComponent)).Should().Be.True();
			inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Any)).Should().Be.True();
		}

		[Test]
		public void WhenDefineEmptyNaturalIdThenNoMapIt()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.NaturalId(nidm =>{});
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			hbmClass.naturalid.Should().Be.Null();
		}
	}
}