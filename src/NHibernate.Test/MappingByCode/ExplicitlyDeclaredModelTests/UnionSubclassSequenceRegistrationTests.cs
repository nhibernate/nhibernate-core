using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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
			inspector.IsTablePerConcreteClass(typeof(Inherited1)).Should().Be.True();
		}

		[Test]
		public void WhenRegisterUnionSubclassWithNoRootThenThrows()
		{
			var inspector = new ExplicitlyDeclaredModel();
			inspector.AddAsTablePerConcreteClassEntity(typeof(Inherited1));

			inspector.Executing(x => x.IsTablePerConcreteClass(typeof(Inherited1))).Throws<MappingException>();
		}		
	}
}