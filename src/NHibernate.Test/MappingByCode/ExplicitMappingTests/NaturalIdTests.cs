using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Name)), Is.True);
			Assert.That(inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Related)), Is.True);
			Assert.That(inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.MyComponent)), Is.True);
			Assert.That(inspector.IsMemberOfNaturalId(For<MyClass>.Property(x => x.Any)), Is.True);
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
			Assert.That(hbmClass.naturalid, Is.Null);
		}
	}
}