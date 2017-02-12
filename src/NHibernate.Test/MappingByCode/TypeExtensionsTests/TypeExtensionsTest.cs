using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class TypeExtensionsTest
	{
		private const BindingFlags BindingFlagsIncludePrivate = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		[Test]
		public void CanDetermineDictionaryKeyType()
		{
			Assert.That(typeof (IDictionary<string, int>).DetermineDictionaryKeyType(), Is.EqualTo(typeof(string)));
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryKeyType()
		{
			Assert.That(typeof(IEnumerable<string>).DetermineDictionaryKeyType(), Is.Null);
		}

		[Test]
		public void CanDetermineDictionaryValueType()
		{
			Assert.That(typeof(IDictionary<string, int>).DetermineDictionaryValueType(), Is.EqualTo(typeof(int)));
		}

		[Test]
		public void WhenNoGenericDictionaryThenDetermineNullDictionaryValueType()
		{
			Assert.That(typeof(IEnumerable<string>).DetermineDictionaryValueType(), Is.Null);
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
			var mi1 = TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseProperty);
			Assert.That(mi1.DeclaringType, Is.EqualTo(typeof (MyBaseClass)));
			Assert.That(mi1.ReflectedType, Is.EqualTo(typeof (MyBaseClass)));

			var mi2 = TypeExtensions.DecodeMemberAccessExpression<MyClass>(mc => mc.BaseBool);
			Assert.That(mi2.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi2.ReflectedType, Is.EqualTo(typeof(MyBaseClass)));
		}

		[Test]
		public void GenericDecodeMemberAccessExpressionShouldReturnMemberOfDeclaringClass()
		{
			var mi1 = TypeExtensions.DecodeMemberAccessExpression<MyClass, string>(mc => mc.BaseProperty);
			Assert.That(mi1.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi1.ReflectedType, Is.EqualTo(typeof(MyBaseClass)));

			var mi2 = TypeExtensions.DecodeMemberAccessExpression<MyClass, bool>(mc => mc.BaseBool);
			Assert.That(mi2.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi2.ReflectedType, Is.EqualTo(typeof(MyBaseClass)));
		}

		[Test]
		public void DecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			var mi1 = TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseProperty);
			Assert.That(mi1.DeclaringType, Is.EqualTo(typeof (MyBaseClass)));
			Assert.That(mi1.ReflectedType, Is.EqualTo(typeof (MyClass)));

			var mi2 = TypeExtensions.DecodeMemberAccessExpressionOf<MyClass>(mc => mc.BaseBool);
			Assert.That(mi2.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi2.ReflectedType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void GenericDecodeMemberAccessExpressionOfShouldReturnMemberOfRequiredClass()
		{
			var mi1 = TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, string>(mc => mc.BaseProperty);
			Assert.That(mi1.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi1.ReflectedType, Is.EqualTo(typeof(MyClass)));

			var mi2 = TypeExtensions.DecodeMemberAccessExpressionOf<MyClass, bool>(mc => mc.BaseBool);
			Assert.That(mi2.DeclaringType, Is.EqualTo(typeof(MyBaseClass)));
			Assert.That(mi2.ReflectedType, Is.EqualTo(typeof(MyClass)));
		}

		[Test]
		public void GetBaseTypesIncludesInterfaces()
		{
			Assert.That(typeof (Collection<>).GetBaseTypes(), Contains.Item(typeof (IEnumerable)));
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
			var mi1 = TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.Id);
			Assert.That(mi1.DeclaringType, Is.EqualTo(typeof (BaseEntity)));
			Assert.That(mi1.ReflectedType, Is.EqualTo(typeof (MyEntity)));

			var mi2 = TypeExtensions.DecodeMemberAccessExpressionOf<MyEntity>(mc => mc.BaseBool);
			Assert.That(mi2.DeclaringType, Is.EqualTo(typeof(BaseEntity)));
			Assert.That(mi2.ReflectedType, Is.EqualTo(typeof(MyEntity)));
		}

		[Test]
		public void WhenBaseIsAbstractGenericGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("Id", typeof(int));
			var declaringMi = mi.GetMemberFromDeclaringType();
			Assert.That(declaringMi.DeclaringType, Is.EqualTo(typeof(BaseEntity)));
			Assert.That(declaringMi.ReflectedType, Is.EqualTo(typeof(BaseEntity)));
		}

		[Test]
		public void WhenBaseIsAbstractGetMemberFromDeclaringType()
		{
			var mi = typeof(MyEntity).GetProperty("BaseBool", typeof(bool));
			var declaringMi = mi.GetMemberFromDeclaringType();
			Assert.That(declaringMi.DeclaringType, Is.EqualTo(typeof(BaseEntity)));
			Assert.That(declaringMi.ReflectedType, Is.EqualTo(typeof(BaseEntity)));
		}

		[Test]
		public void GetFirstPropertyOfTypeWithNulls()
		{
			System.Type myType = null;
			Assert.That(myType.GetFirstPropertyOfType(typeof (int), BindingFlagsIncludePrivate), Is.Null);
			myType = typeof (Array);
			Assert.That(myType.GetFirstPropertyOfType(null, BindingFlagsIncludePrivate), Is.Null);
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyExistThenFindProperty()
		{
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string)), Is.EqualTo(typeof (MyBaseClass).GetProperty("BaseProperty")));
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (bool)), Is.EqualTo(typeof (MyBaseClass).GetProperty("BaseBool")));
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double), BindingFlagsIncludePrivate), Is.EqualTo(typeof (MyBaseClass).GetProperty("SomethingPrivate", BindingFlagsIncludePrivate)));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenPropertyNotExistThenNull()
		{
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (float)), Is.Null);
			//Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (double)), Is.Null); <= by default check private prop.
		}

		private interface IMyEntity : IEntity<Guid>
		{

		}

		[Test]
		public void WhenDecodeMemberAccessExpressionOfOnInheritedEntityInterfaceThenDecodeMember()
		{
			Assert.That(TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity>(m => m.Id), Is.Not.Null);
			Assert.That(TypeExtensions.DecodeMemberAccessExpressionOf<IMyEntity, Guid>(m => m.Id), Is.Not.Null);
		}

		[Test]
		public void TheSequenceOfGetHierarchyFromBaseShouldStartFromBaseClassUpToGivenClass()
		{
			// excluding System.Object
			var expected = new[] {typeof (AbstractEntity<int>), typeof (BaseEntity), typeof (MyEntity)};
			Assert.That(typeof (MyEntity).GetHierarchyFromBase(), Is.EquivalentTo(expected));
		}

		[Test]
		public void GetFirstPropertyOfType_WhenDelegateIsNullThenThrow()
		{
			var myType = typeof(Array);
			Assert.That(() => myType.GetFirstPropertyOfType(typeof(int), BindingFlagsIncludePrivate, null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void GetFirstPropertyOfType_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => false), Is.Null);
			Assert.That(typeof (MyBaseClass).GetFirstPropertyOfType(typeof (string), BindingFlags.Public | BindingFlags.Instance, x => true), Is.EqualTo(typeof (MyBaseClass).GetProperty("BaseProperty")));
		}

		[Test]
		public void HasPublicPropertyOf_WhenAsDelegateThenUseDelegateToFilterProperties()
		{
			Assert.That(typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => false), Is.False);
			Assert.That(typeof(MyBaseClass).HasPublicPropertyOf(typeof(string), x => true), Is.True);
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
			Assert.That(found.Count, Is.EqualTo(2));
			var concreteMember = For<MyConcrete>.Property(x => x.Description).GetMemberFromReflectedType(typeof(MyConcrete));
			var abstractMember = For<MyAbstract>.Property(x => x.Description);
			Assert.That(found, Is.EquivalentTo(new [] {concreteMember, abstractMember}));
		}

		[Test]
		public void GetMemberFromDeclaringClasses_WhenFieldThenFindAbstract()
		{
			var member = typeof(MyConcrete).GetField("aField", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			var found = member.GetMemberFromDeclaringClasses().ToList();
			Assert.That(found.Count, Is.EqualTo(1));
			var foundMember = found.Single();
			Assert.That(foundMember.DeclaringType, Is.EqualTo(typeof(MyAbstract)));
			Assert.That(foundMember.ReflectedType, Is.EqualTo(typeof(MyAbstract)));
		}

		private class MyCustomCollection : List<MyEntity>
		{
		}

		[Test]
		public void DetermineCollectionElementTypeShouldDetermineElementTypeWhenCollectionTypeIsGeneric()
		{
			var elementType = typeof(List<MyEntity>).DetermineCollectionElementType();
			Assert.That(elementType, Is.EqualTo(typeof(MyEntity)));
		}

		[Test(Description = "NH-3054")]
		public void DetermineCollectionElementTypeShouldDetermineElementTypeWhenCollectionTypeIsNonGeneric()
		{
			var elementType = typeof(MyCustomCollection).DetermineCollectionElementType();
			Assert.That(elementType, Is.EqualTo(typeof(MyEntity)));
		}

		[Test]
		public void DetermineCollectionElementTypeShouldNotDetermineElementTypeWhenTypeIsNotACollection()
		{
			var elementType = typeof(MyEntity).DetermineCollectionElementType();
			Assert.That(elementType, Is.Null);
		}
	}
}