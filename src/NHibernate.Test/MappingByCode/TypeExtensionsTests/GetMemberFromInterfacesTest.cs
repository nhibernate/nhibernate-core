using System;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class GetMemberFromInterfacesTest
	{
		private class BaseEntity
		{
			public int Id { get; set; }
		}

		private interface IEntity
		{
			bool IsValid { get; }
			string Something { get; set; }
		}

		private interface IHasSomething
		{
			string Something { get; set; }
		}

		private class Person : BaseEntity, IEntity, IHasSomething
		{
			private int someField;
			public string Name { get; set; }
			public bool IsValid { get { return false; } }
			public string Something { get; set; }
		}

		private interface IInheritedHasSomething : IHasSomething
		{
			string Blah { get; set; }
		}


		[Test]
		public void WhenNullArgumentThenThrows()
		{
			Assert.That(() => ((MemberInfo)null).GetPropertyFromInterfaces().ToList(), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenNoInterfaceThenEmptyList()
		{
			Assert.That(For<BaseEntity>.Property(x=> x.Id).GetPropertyFromInterfaces(), Is.Empty);
		}

		[Test]
		public void WhenFieldThenEmptyList()
		{
			Assert.That(ForClass<Person>.Field("someField").GetPropertyFromInterfaces(), Is.Empty);
		}

		[Test]
		public void WhenOneInterfaceThenReturnMemberInfoOfInterface()
		{
			var members = For<Person>.Property(x => x.IsValid).GetPropertyFromInterfaces();
			Assert.That(members.Single(), Is.EqualTo(For<IEntity>.Property(x=> x.IsValid)));
		}

		[Test]
		public void WhenTwoInterfacesThenReturnMemberInfoOfEachInterface()
		{
			var members = For<Person>.Property(x => x.Something).GetPropertyFromInterfaces();
			Assert.That(members, Contains.Item(For<IEntity>.Property(x => x.Something)));
			Assert.That(members, Contains.Item(For<IHasSomething>.Property(x => x.Something)));
		}

		[Test]
		public void WhenPropertyOfInterfaceThenNotThrows()
		{
			var member = For<IInheritedHasSomething>.Property(x => x.Blah);
			Assert.That(() => member.GetPropertyFromInterfaces().Any(), Throws.Nothing);
		}
	}
}