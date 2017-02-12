using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class GetMemberFromReflectedTest
	{
		private const BindingFlags PrivateMembersFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		private class MyClass
		{
			private int pField;
			private int PrivateProperty { get; set; }
			public int AnotherProperty { get; set; }
			protected int ProtectedProperty { get; set; }
			private int Method() { return 0; }
		}

		private class Inherited : MyClass
		{
			private int pField;
			private int PrivateProperty { get; set; }
		}

		private interface IInterface
		{
			int Something { get; set; }
			int SomethingElse { get; set; }
		}

		private class MyClassWithExplicitImpl : IInterface
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
		public void WhenNullMemberThenThrows()
		{
			Assert.That(() => ((MemberInfo)null).GetMemberFromReflectedType(typeof(MyClass)), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenNullTypeThenThrows()
		{
			Assert.That(() => typeof(MyClassWithExplicitImpl).GetProperty("SomethingElse").GetMemberFromReflectedType(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenNotExistentThenOriginal()
		{
			var propertyInfo = typeof(MyClassWithExplicitImpl).GetProperty("SomethingElse");
			Assert.That(propertyInfo.GetMemberFromReflectedType(typeof(object)), Is.EqualTo(propertyInfo));
		}

		[Test]
		public void WhenNotAccessibleFieldThenOriginal()
		{
			var memberInfo = typeof(MyClass).GetField("pField", PrivateMembersFlags);
			Assert.That(memberInfo.GetMemberFromReflectedType(typeof(Inherited)), Is.EqualTo(memberInfo));
		}

		[Test]
		public void WhenNotAccessiblePropertyThenOriginal()
		{
			var memberInfo = typeof(MyClass).GetProperty("PrivateProperty", PrivateMembersFlags);
			Assert.That(memberInfo.GetMemberFromReflectedType(typeof(Inherited)), Is.EqualTo(memberInfo));
		}

		[Test]
		public void WhenAccessiblePropertyThenReflected()
		{
			var memberInfo = typeof(MyClass).GetProperty("ProtectedProperty", PrivateMembersFlags);
			var result = memberInfo.GetMemberFromReflectedType(typeof(Inherited));
			Assert.That(result.ReflectedType, Is.EqualTo(typeof(Inherited)));
			Assert.That(result.DeclaringType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void WhenPrivateFieldOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetField("pField", PrivateMembersFlags);
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClass));
			Assert.That(result.ReflectedType, Is.EqualTo(typeof(Inherited)));
			Assert.That(result.DeclaringType, Is.EqualTo(typeof(Inherited)));
		}

		[Test]
		public void WhenPublicPropertyOfBaseOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(MyClass).GetProperty("AnotherProperty");
			var result = memberInfo.GetMemberFromReflectedType(typeof(Inherited));
			Assert.That(result.ReflectedType, Is.EqualTo(typeof(Inherited)));
			Assert.That(result.DeclaringType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void WhenPropertyOfInterfaceThenFindItOnClass()
		{
			var memberInfo = typeof(IInterface).GetProperty("SomethingElse");
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClassWithExplicitImpl));
			Assert.That(result.DeclaringType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
			Assert.That(result.ReflectedType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
		}

		[Test]
		public void WhenPropertyOfExplicitInterfaceThenFindItOnClass()
		{
			var memberInfo = typeof(IInterface).GetProperty("Something");
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClassWithExplicitImpl));
			Assert.That(result.DeclaringType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
			Assert.That(result.ReflectedType, Is.EqualTo(typeof(MyClassWithExplicitImpl)));
		}
	}
}