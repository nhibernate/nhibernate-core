using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class SplitPropertiesRegistrationTests
	{
		private class MyClass
		{
			public string Something { get; set; }
		}

		private class Inherited : MyClass
		{
			public string SomethingElse { get; set; }
		}

		[Test]
		public void WhenRegisterPropertySplitsThenTypeHasSplitGroups()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsPropertySplit(new SplitDefinition(typeof(MyClass), "group", For<MyClass>.Property(x => x.Something)));
			inspector.AddAsPropertySplit(new SplitDefinition(typeof(Inherited), "group1", For<Inherited>.Property(x => x.SomethingElse)));

			inspector.GetSplitGroupsFor(typeof(MyClass)).Should().Have.SameValuesAs("group");
			inspector.GetSplitGroupsFor(typeof(Inherited)).Should().Have.SameValuesAs("group1");
		}

		[Test]
		public void WhenRegisterPropertySplitMoreThanOnceThenIgnore()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var memberFromDeclaringType = For<MyClass>.Property(x=> x.Something);
			var memberFromReferencedType = typeof(Inherited).GetProperty("Something");

			inspector.AddAsPropertySplit(new SplitDefinition(typeof(MyClass), "group", memberFromDeclaringType));
			inspector.AddAsPropertySplit(new SplitDefinition(typeof(Inherited), "group1", memberFromReferencedType));

			inspector.GetSplitGroupsFor(typeof(MyClass)).Should().Have.SameValuesAs("group");
			inspector.GetSplitGroupsFor(typeof(Inherited)).Should().Be.Empty();

			inspector.GetSplitGroupFor(memberFromDeclaringType).Should().Be("group");
			inspector.GetSplitGroupFor(memberFromReferencedType).Should().Be("group");
		}
	}
}