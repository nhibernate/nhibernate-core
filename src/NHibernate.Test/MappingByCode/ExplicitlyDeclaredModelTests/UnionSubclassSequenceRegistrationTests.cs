using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitlyDeclaredModelTests
{
	public class UnionSubclassSequenceRegistrationTests
	{
		private class MyClass
		{

		}
		private class Inherited1 : MyClass
		{

		}

		[Test]
		public void WhenRegisterUnionSubclassBeforeRootThenIsRegistered()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			inspector.AddAsRootEntity(typeof(MyClass));
			Assert.That(inspector.IsTablePerConcreteClass(typeof(Inherited1)), Is.True);
		}

		[Test]
		public void WhenRegisterUnionSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			Assert.That(() => inspector.IsTablePerConcreteClass(typeof(Inherited1)), Throws.TypeOf<MappingException>());
		}		
	}
}