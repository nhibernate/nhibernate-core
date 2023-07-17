using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public sealed class CharBooleanTypeFixture
	{
		[Theory]
		public void GetByIndex(bool expected)
		{
			const int index0 = 0;
			const int index1 = 1;
			CharBooleanType type =new CharBooleanTypeStub();
			var session = Substitute.For<ISessionImplementor>();
			var reader = Substitute.For<DbDataReader>();
			reader[index0].Returns(expected.ToString());
			reader[index1].Returns(expected.ToString());

			var result0 = type.Get(reader, index0, session);
			var result1 = type.Get(reader, index1, session);

			Assert.AreEqual(expected, (bool) result0);
			Assert.AreSame(result0, result1);
		}

		[Theory]
		public void GetByName(bool value)
		{
			const string name = "0";
			const int index = 0;
			object expected = value;
			CharBooleanType type = Substitute.ForPartsOf<CharBooleanTypeStub>();
			var session = Substitute.For<ISessionImplementor>();
			var reader = Substitute.For<DbDataReader>();
			reader.GetOrdinal(name).Returns(index);
			type.Get(reader, index, Arg.Any<ISessionImplementor>()).Returns(expected);

			var result = type.Get(reader, name, session);

			Assert.AreSame(expected, result);
		}

		[Theory]
		public void StringToObject(bool expected)
		{
			CharBooleanType type = new CharBooleanTypeStub();

			var result0 = type.StringToObject(expected.ToString());
			var result1 = type.StringToObject(expected.ToString());

			Assert.AreEqual(expected, result0);
			Assert.AreSame(result0, result1);
		}

		public class CharBooleanTypeStub : CharBooleanType
		{
			public CharBooleanTypeStub() : base(new AnsiStringFixedLengthSqlType())
			{
			}

			protected override string TrueString => true.ToString();

			protected override string FalseString => false.ToString();
		}
	}
}
