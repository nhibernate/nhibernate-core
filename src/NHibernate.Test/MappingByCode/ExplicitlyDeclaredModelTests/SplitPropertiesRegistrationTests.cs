using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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

			Assert.That(inspector.GetSplitGroupsFor(typeof(MyClass)), Is.EquivalentTo(new [] {"group"}));
			Assert.That(inspector.GetSplitGroupsFor(typeof(Inherited)), Is.EquivalentTo(new [] {"group1"}));
		}

		[Test]
		public void WhenRegisterPropertySplitMoreThanOnceThenIgnore()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var memberFromDeclaringType = For<MyClass>.Property(x=> x.Something);
			var memberFromReferencedType = typeof(Inherited).GetProperty("Something");

			inspector.AddAsPropertySplit(new SplitDefinition(typeof(MyClass), "group", memberFromDeclaringType));
			inspector.AddAsPropertySplit(new SplitDefinition(typeof(Inherited), "group1", memberFromReferencedType));

			Assert.That(inspector.GetSplitGroupsFor(typeof(MyClass)), Is.EquivalentTo(new [] {"group"}));
			Assert.That(inspector.GetSplitGroupsFor(typeof(Inherited)), Is.Empty);

			Assert.That(inspector.GetSplitGroupFor(memberFromDeclaringType), Is.EqualTo("group"));
			Assert.That(inspector.GetSplitGroupFor(memberFromReferencedType), Is.EqualTo("group"));
		}
	}
}