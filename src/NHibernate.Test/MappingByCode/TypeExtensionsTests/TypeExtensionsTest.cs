using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class TypeExtensionsTest
	{
		private const BindingFlags BindingFlagsIncludePrivate = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		[Test]
		public void CanDetermineDictionaryKeyType()
		{
			typeof (IDictionary<string, int>).DetermineDictionaryKeyType().Should().Be.EqualTo<string>();
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryKeyType()
		{
			typeof(IEnumerable<string>).DetermineDictionaryKeyType().Should().Be.Null();
		}

		[Test]
		public void CanDetermineDictionaryValueType()
		{
			typeof(IDictionary<string, int>).DetermineDictionaryValueType().Should().Be.EqualTo<int>();
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryValueType()
		{
			typeof(IEnumerable<string>).DetermineDictionaryValueType().Should().Be.Null();
		}

		private class MyBaseClass
		{
			public string BaseProperty { get; set; }
			public bool BaseBool { get; set; }
			private double SomethingPrivate { get; set; }
		}
		private class MyClass : MyBaseClass
		{
			
		}

		[Test]
		public void DecodeMemberAccessExpressionShouldReturnMemberOfDeclaringClass()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void GenericDecodeMemberAccessExpressionShouldReturnMemberOfDeclaringClass()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass, string>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression<MyClass, bool>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyBaseClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void DecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof (MyClass) && mi.DeclaringType == typeof (MyBaseClass));
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void GenericDecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, string>(mc => mc.BaseProperty).Satisfy(
				mi => mi.ReflectedType == typeof(MyClass) && mi.DeclaringType == typeof(MyBaseClass));
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, bool>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyClass) && mi.DeclaringType == typeof(MyBaseClass));
		}

		[Test]
		public void GetBaseTypesIncludesInterfaces()
		{
			typeof (Collection<>).GetBaseTypes().Should().Contain(typeof (IEnumerable));
		}

		private interface IEntity<T>
		{
			T Id { get; set; }
		}
		private abstract class AbstractEntity<T> : IEntity<T>
		{
			public abstract T Id { get; set; }
			public abstract bool BaseBool { get; set; }
		}

		private class BaseEntity : AbstractEntity<int>
		{
			public override int Id { get; set; }

			public override bool BaseBool { get; set; }
		}
		private class MyEntity: BaseEntity
		{
		}

		[Test]
		public void DecodeMemberAccessExpressionOfWithGenericShouldReturnMemberOfRequiredClass()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.Id).Satisfy(
				mi => mi.ReflectedType == typeof(MyEntity) && mi.DeclaringType == typeof(BaseEntity));
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.BaseBool).Satisfy(
				mi => mi.ReflectedType == typeof(MyEntity) && mi.DeclaringType == typeof(BaseEntity));
		}

		[Test]
		public void WhenBaseIsAbstractGenericGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("Id", typeof(int));
			var declaringMi = mi.GetMemberFromDeclaringType();
			declaringMi.DeclaringType.Should().Be<BaseEntity>();
			declaringMi.ReflectedType.Should().Be<BaseEntity>();
		}

		[Test]
		public void WhenBaseIsAbstractGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("BaseBool", typeof(bool));
			var declaringMi = mi.GetMemberFromDeclaringType();
			declaringMi.DeclaringType.Should().Be<BaseEntity>();
			declaringMi.ReflectedType.Should().Be<BaseEntity>();
		}

		[Test]
		public void GetFirstPropertyOfTypeWithNulls()
		{
			System.Type myType = null;
			myType.GetFirstPropertyOfType(typeof (int), BindingFlagsIncludePrivate).Should().Be.Null();
			myType = typeof (Array);
			myType.GetFirstPropertyOfType(null, BindingFlagsIncludePrivate).Should().Be.Null();
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyExistThenFindProperty()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string)).Should().Be(
				typeof (MyBaseClass).GetProperty("BaseProperty"));
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (bool)).Should().Be(typeof (MyBaseClass).GetProperty("BaseBool"));
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double), BindingFlagsIncludePrivate).Should().Be(
				typeof (MyBaseClass).GetProperty("SomethingPrivate", BindingFlagsIncludePrivate));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyNotExistThenNull()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (float)).Should().Be.Null();
			// typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double)).Should().Be.Null(); <= by default check private prop.
		}

		private interface IMyEntity : IEntity<Guid>
		{

		}

		[Test]
		public void WhenDecodeMemberAccessExpressionOfOnInheritedEntityInterfaceThenDecodeMember()
		{
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity>(m => m.Id).Should().Not.Be.Null();
			Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity, Guid>(m => m.Id).Should().Not.Be.Null();
		}

		[Test]
		public void TheSequenceOfGetHierarchyFromBaseShouldStartFromBaseClassUpToGivenClass()
		{
			// excluding System.Object
			typeof(MyEntity).GetHierarchyFromBase().Should().Have.SameSequenceAs(typeof(AbstractEntity<int>), typeof(BaseEntity), typeof(MyEntity));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenDelegateIsNullThenThrow()
		{
			var myType = typeof(Array);
			Executing.This(()=> myType.GetFirstPropertyOfType(typeof(int), BindingFlagsIncludePrivate, null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void GetFirstPropertyOfType_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => false).Should().Be.Null();
			typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => true).Should().Be(
				typeof (MyBaseClass).GetProperty("BaseProperty"));
		}

		[Test]
		public void HasPublicPropertyOf_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => false).Should().Be.False();
			typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => true).Should().Be.True();
		}

		private abstract class MyAbstract
		{
			protected int aField;
			public abstract string Description { get; }
		}

		private class MyConcrete : MyAbstract
		{
			public override string Description
			{
				get { return "blah"; }
			}
		}

		[Test]
		public void GetMemberFromDeclaringClasses_WhenPropertyThenFindAbstract()
		{
			var member = typeof(MyConcrete).GetProperty("Description", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var found = member.GetMemberFromDeclaringClasses().ToList();
			found.Count.Should().Be(2);
			var concreteMember = For<MyConcrete>.Property(x => x.Description).GetMemberFromReflectedType(typeof(MyConcrete));
			var abstractMember = For<MyAbstract>.Property(x => x.Description);
			found.Should().Have.SameValuesAs(concreteMember, abstractMember);
		}

		[Test]
		public void GetMemberFromDeclaringClasses_WhenFieldThenFindAbstract()
		{
			var member = typeof(MyConcrete).GetField("aField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var found = member.GetMemberFromDeclaringClasses().ToList();
			found.Count.Should().Be(1);
			var foundMember = found.Single();
			foundMember.DeclaringType.Should().Be(typeof(MyAbstract));
			foundMember.ReflectedType.Should().Be(typeof(MyAbstract));
		}

		private class MyCustomCollection : List<MyEntity>
		{
		}

		[Test]
		public void DetermineCollectionElementTypeShouldDetermineElementTypeWhenCollectionTypeIsGeneric()
		{
			var elementType = typeof(List<MyEntity>).DetermineCollectionElementType();
			elementType.Should().Be(typeof(MyEntity));
		}

		[Test(Description = "NH-3054")]
		public void DetermineCollectionElementTypeShouldDetermineElementTypeWhenCollectionTypeIsNonGeneric()
		{
			var elementType = typeof(MyCustomCollection).DetermineCollectionElementType();
			elementType.Should().Be(typeof(MyEntity));
		}

		[Test]
		public void DetermineCollectionElementTypeShouldNotDetermineElementTypeWhenTypeIsNotACollection()
		{
			var elementType = typeof(MyEntity).DetermineCollectionElementType();
			elementType.Should().Be.Null();
		}
	}
}