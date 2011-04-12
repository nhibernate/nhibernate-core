using System.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class JoinedSubclassKeyAsRootIdColumnTest
	{
		private class MyClass
		{
			public int MyId { get; set; }
			public int HisId { get; set; }
		}

		private class Inherited : MyClass
		{
		}

		[Test]
		public void WhenBaseIsIdThenUseId()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(map=> map.Id(x=> x.MyId));
			var mapping = mapper.CompileMappingFor(new[] { typeof(Inherited), typeof(MyClass) });
			var hbmJoinedClass = mapping.JoinedSubclasses[0];

			hbmJoinedClass.key.Columns.Single().name.Should().Be("MyId");
		}

		[Test]
		public void WhenBaseIsPoIdThenUsePoId()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(map => map.Id(x => x.HisId));
			var mapping = mapper.CompileMappingFor(new[] { typeof(Inherited), typeof(MyClass) });
			var hbmJoinedClass = mapping.JoinedSubclasses[0];

			hbmJoinedClass.key.Columns.Single().name.Should().Be("HisId");
		}

		[Test]
		public void WhenNoPoidMemberThenDoesNotSet()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(map => { });
			var mapping = mapper.CompileMappingFor(new[] { typeof(Inherited), typeof(MyClass) });
			var hbmJoinedClass = mapping.JoinedSubclasses[0];

			hbmJoinedClass.key.Columns.Single().name.Should().Not.Be("MyId").And.Not.Be("HisId");
		}
	}
}