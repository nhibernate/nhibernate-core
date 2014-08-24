using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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
			Executing.This(() => ((MemberInfo)null).GetMemberFromReflectedType(typeof(MyClass))).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNullTypeThenThrows()
		{
			Executing.This(() => typeof(MyClassWithExplicitImpl).GetProperty("SomethingElse").GetMemberFromReflectedType(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNotExistentThenOriginal()
		{
			var propertyInfo = typeof(MyClassWithExplicitImpl).GetProperty("SomethingElse");
			propertyInfo.GetMemberFromReflectedType(typeof(object)).Should().Be(propertyInfo);
		}

		[Test]
		public void WhenNotAccessibleFieldThenOriginal()
		{
			var memberInfo = typeof(MyClass).GetField("pField", PrivateMembersFlags);
			memberInfo.GetMemberFromReflectedType(typeof(Inherited)).Should().Be(memberInfo);
		}

		[Test]
		public void WhenNotAccessiblePropertyThenOriginal()
		{
			var memberInfo = typeof(MyClass).GetProperty("PrivateProperty", PrivateMembersFlags);
			memberInfo.GetMemberFromReflectedType(typeof(Inherited)).Should().Be(memberInfo);
		}

		[Test]
		public void WhenAccessiblePropertyThenReflected()
		{
			var memberInfo = typeof(MyClass).GetProperty("ProtectedProperty", PrivateMembersFlags);
			var result = memberInfo.GetMemberFromReflectedType(typeof(Inherited));
			result.ReflectedType.Should().Be(typeof(Inherited));
			result.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenPrivateFieldOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(Inherited).GetField("pField", PrivateMembersFlags);
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClass));
			result.ReflectedType.Should().Be(typeof(Inherited));
			result.DeclaringType.Should().Be(typeof(Inherited));
		}

		[Test]
		public void WhenPublicPropertyOfBaseOnInheritedThenFindItOnInherited()
		{
			var memberInfo = typeof(MyClass).GetProperty("AnotherProperty");
			var result = memberInfo.GetMemberFromReflectedType(typeof(Inherited));
			result.ReflectedType.Should().Be(typeof(Inherited));
			result.DeclaringType.Should().Be(typeof(MyClass));
		}

		[Test]
		public void WhenPropertyOfInterfaceThenFindItOnClass()
		{
			var memberInfo = typeof(IInterface).GetProperty("SomethingElse");
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClassWithExplicitImpl));
			result.DeclaringType.Should().Be(typeof(MyClassWithExplicitImpl));
			result.ReflectedType.Should().Be(typeof(MyClassWithExplicitImpl));
		}

		[Test]
		public void WhenPropertyOfExplicitInterfaceThenFindItOnClass()
		{
			var memberInfo = typeof(IInterface).GetProperty("Something");
			var result = memberInfo.GetMemberFromReflectedType(typeof(MyClassWithExplicitImpl));
			result.DeclaringType.Should().Be(typeof(MyClassWithExplicitImpl));
			result.ReflectedType.Should().Be(typeof(MyClassWithExplicitImpl));
		}
	}
}