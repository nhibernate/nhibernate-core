using System;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

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
			Executing.This(() => ((MemberInfo)null).GetPropertyFromInterfaces().ToList()).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenNoInterfaceThenEmptyList()
		{
			For<BaseEntity>.Property(x=> x.Id).GetPropertyFromInterfaces().Should().Be.Empty();
		}

		[Test]
		public void WhenFieldThenEmptyList()
		{
			ForClass<Person>.Field("someField").GetPropertyFromInterfaces().Should().Be.Empty();
		}

		[Test]
		public void WhenOneInterfaceThenReturnMemberInfoOfInterface()
		{
			var members = For<Person>.Property(x => x.IsValid).GetPropertyFromInterfaces();
			members.Single().Should().Be(For<IEntity>.Property(x=> x.IsValid));
		}

		[Test]
		public void WhenTwoInterfacesThenReturnMemberInfoOfEachInterface()
		{
			var members = For<Person>.Property(x => x.Something).GetPropertyFromInterfaces();
			members.Should().Contain(For<IEntity>.Property(x => x.Something));
			members.Should().Contain(For<IHasSomething>.Property(x => x.Something));
		}

		[Test]
		public void WhenPropertyOfInterfaceThenNotThrows()
		{
			var member = For<IInheritedHasSomething>.Property(x => x.Blah);
			member.Executing(x=> x.GetPropertyFromInterfaces().Any()).NotThrows();
		}
	}
}