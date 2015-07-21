using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class PoidTests
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenPropertyUsedAsPoidThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(map => map.Id(x => x.Id, idmap => { }));

			Assert.That(inspector.IsPersistentId(For<MyClass>.Property(x => x.Id)), Is.True);
		}
	}
}