using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;
namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class GetPropertyOrFieldMatchingNameTest
	{
		private class MyClass
		{
			private int pField;
			private int PrivateProperty { get; set; }
			private int AnotherPrivateProperty { get; set; }
			protected int ProtectedProperty { get; set; }
			private int Method() { return 0; }
		}

		private class Inherited: MyClass
		{
			private int pField;
			private int PrivateProperty { get; set; }
		}

		private interface IInterface
		{
			int Something { get; set; }
			int SomethingElse { get; set; }
		}

		private class MyClassWithExplicitImpl: IInterface
		{
			int IInterface.Something
			{
				get
				{
					throw new System.NotImplementedException();
				}
				set
				{
					throw new System.NotImplementedException();
				}
			}

			public int SomethingElse
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}
		}

		[Test]
		public void WhenNullTypeThenThrows()
		{
			Executing.This(() => ((System.Type)null).GetPropertyOrFieldMatchingName("A")).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenAskNullThenNull()
		{
			typeof(MyClass).GetPropertyOrFieldMatchingName(null).Should().Be.Null();
		}

		[Test]
		public void WhenAskNotExistentThenNull()
		{
			typeof(MyClass).GetPropertyOrFieldMatchingName("NotExistent").Should().Be.Null();
		}

		[Test]
		public void WhenAskPrivateFieldThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("pField");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("pField");
			memberInfo.Should().Be.InstanceOf<FieldInfo>().And.ValueOf.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenAskPrivateFieldWithBlanksThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("   pField   ");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("pField");
			memberInfo.Should().Be.InstanceOf<FieldInfo>().And.ValueOf.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenAskPrivatePropertyThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("PrivateProperty");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("PrivateProperty");
			memberInfo.Should().Be.InstanceOf<PropertyInfo>().And.ValueOf.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenAskProtectedPropertyThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("ProtectedProperty");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("ProtectedProperty");
			memberInfo.Should().Be.InstanceOf<PropertyInfo>().And.ValueOf.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenAskMethodThenNull()
		{
			typeof(MyClass).GetPropertyOrFieldMatchingName("Method").Should().Be.Null();
		}

		[Test]
		public void WhenAskPrivateFieldOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("pField");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("pField");
			memberInfo.DeclaringType.Should().Be(typeof(Inherited));
			memberInfo.ReflectedType.Should().Be(typeof(Inherited));
			memberInfo.Should().Be.InstanceOf<FieldInfo>();
		}

		[Test]
		public void WhenAskPrivatePropertyOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("PrivateProperty");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("PrivateProperty");
			memberInfo.DeclaringType.Should().Be(typeof(Inherited));
			memberInfo.ReflectedType.Should().Be(typeof(Inherited));
			memberInfo.Should().Be.InstanceOf<PropertyInfo>();
		}

		[Test]
		public void WhenAskPrivatePropertyOfBaseOnInheritedThenFindItOnBase()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("AnotherPrivateProperty");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("AnotherPrivateProperty");
			memberInfo.DeclaringType.Should().Be(typeof(MyClass));
			memberInfo.ReflectedType.Should().Be(typeof(MyClass));
			memberInfo.Should().Be.InstanceOf<PropertyInfo>();
		}

		[Test]
		public void WhenAskPropertyOfInterfaceThenFindIt()
		{
			var memberInfo = typeof(IInterface).GetPropertyOrFieldMatchingName("Something");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("Something");
			memberInfo.DeclaringType.Should().Be(typeof(IInterface));
			memberInfo.ReflectedType.Should().Be(typeof(IInterface));
			memberInfo.Should().Be.InstanceOf<PropertyInfo>();
		}

		[Test]
		public void WhenAskPropertyOfExplicitInterfaceThenFindItOnInterface()
		{
			var memberInfo = typeof(MyClassWithExplicitImpl).GetPropertyOrFieldMatchingName("Something");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("Something");
			memberInfo.DeclaringType.Should().Be(typeof(IInterface));
			memberInfo.ReflectedType.Should().Be(typeof(IInterface));
			memberInfo.Should().Be.InstanceOf<PropertyInfo>();
		}

		[Test]
		public void WhenAskPropertyOfImplementedInterfaceThenFindItOnType()
		{
			var memberInfo = typeof(MyClassWithExplicitImpl).GetPropertyOrFieldMatchingName("SomethingElse");
			memberInfo.Should().Not.Be.Null();
			memberInfo.Name.Should().Be("SomethingElse");
			memberInfo.DeclaringType.Should().Be(typeof(MyClassWithExplicitImpl));
			memberInfo.ReflectedType.Should().Be(typeof(MyClassWithExplicitImpl));
			memberInfo.Should().Be.InstanceOf<PropertyInfo>();
		}
	}
}