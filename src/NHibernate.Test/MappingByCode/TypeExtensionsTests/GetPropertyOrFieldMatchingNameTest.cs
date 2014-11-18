using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
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
			Assert.That(() => ((System.Type)null).GetPropertyOrFieldMatchingName("A"), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenAskNullThenNull()
		{
			Assert.That(typeof(MyClass).GetPropertyOrFieldMatchingName(null), Is.Null);
		}

		[Test]
		public void WhenAskNotExistentThenNull()
		{
			Assert.That(typeof(MyClass).GetPropertyOrFieldMatchingName("NotExistent"), Is.Null);
		}

		[Test]
		public void WhenAskPrivateFieldThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("pField");
			Assert.That(memberInfo, Is.Not.Null.And.InstanceOf<FieldInfo>());
			Assert.That(memberInfo.Name, Is.EqualTo("pField"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof (MyClass)));
		}

		[Test]
		public void WhenAskPrivateFieldWithBlanksThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("   pField   ");
			Assert.That(memberInfo, Is.Not.Null.And.InstanceOf<FieldInfo>());
			Assert.That(memberInfo.Name, Is.EqualTo("pField"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void WhenAskPrivatePropertyThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("PrivateProperty");
			Assert.That(memberInfo, Is.Not.Null.And.InstanceOf<PropertyInfo>());
			Assert.That(memberInfo.Name, Is.EqualTo("PrivateProperty"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof (MyClass)));
		}

		[Test]
		public void WhenAskProtectedPropertyThenFindIt()
		{
			var memberInfo = typeof(MyClass).GetPropertyOrFieldMatchingName("ProtectedProperty");
			Assert.That(memberInfo, Is.Not.Null.And.InstanceOf<PropertyInfo>());
			Assert.That(memberInfo.Name, Is.EqualTo("ProtectedProperty"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void WhenAskMethodThenNull()
		{
			Assert.That(typeof(MyClass).GetPropertyOrFieldMatchingName("Method"), Is.Null);
		}

		[Test]
		public void WhenAskPrivateFieldOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("pField");
			Assert.That(memberInfo, Is.Not.Null.And.InstanceOf<FieldInfo>());
			Assert.That(memberInfo.Name, Is.EqualTo("pField"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(Inherited)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(Inherited)));
		}

		[Test]
		public void WhenAskPrivatePropertyOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("PrivateProperty");
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("PrivateProperty"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(Inherited)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(Inherited)));
			Assert.That(memberInfo, Is.InstanceOf<PropertyInfo>());
		}

		[Test]
		public void WhenAskPrivatePropertyOfBaseOnInheritedThenFindItOnBase()
		{
			var memberInfo = typeof(Inherited).GetPropertyOrFieldMatchingName("AnotherPrivateProperty");
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("AnotherPrivateProperty"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(MyClass)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(MyClass)));
			Assert.That(memberInfo, Is.InstanceOf<PropertyInfo>());
		}

		[Test]
		public void WhenAskPropertyOfInterfaceThenFindIt()
		{
			var memberInfo = typeof(IInterface).GetPropertyOrFieldMatchingName("Something");
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("Something"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(IInterface)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(IInterface)));
			Assert.That(memberInfo, Is.InstanceOf<PropertyInfo>());
		}

		[Test]
		public void WhenAskPropertyOfExplicitInterfaceThenFindItOnInterface()
		{
			var memberInfo = typeof(MyClassWithExplicitImpl).GetPropertyOrFieldMatchingName("Something");
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("Something"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(IInterface)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(IInterface)));
			Assert.That(memberInfo, Is.InstanceOf<PropertyInfo>());
		}

		[Test]
		public void WhenAskPropertyOfImplementedInterfaceThenFindItOnType()
		{
			var memberInfo = typeof(MyClassWithExplicitImpl).GetPropertyOrFieldMatchingName("SomethingElse");
			Assert.That(memberInfo, Is.Not.Null);
			Assert.That(memberInfo.Name, Is.EqualTo("SomethingElse"));
			Assert.That(memberInfo.DeclaringType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
			Assert.That(memberInfo.ReflectedType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
			Assert.That(memberInfo, Is.InstanceOf<PropertyInfo>());
		}
	}
}