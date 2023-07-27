using System;
using log4net;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Test Fixture for TypeFactory.
	/// </summary>
	[TestFixture]
	public class TypeFactoryFixture
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TypeFactoryFixture));

		/// <summary>
		/// Test that calling GetGuidType multiple times returns the
		/// exact same GuidType object by reference.
		/// </summary>
		[Test]
		public void GetGuidSingleton()
		{
			NullableType guidType = NHibernateUtil.Guid;
			NullableType guidType2 = NHibernateUtil.Guid;

			Assert.AreSame(guidType, guidType2);
		}

		/// <summary>
		/// Test that Strings with different lengths return different StringTypes.
		/// </summary>
		[Test]
		public void GetStringWithDiffLength()
		{
			NullableType string25 = TypeFactory.GetStringType(25);
			NullableType string30 = TypeFactory.GetStringType(30);

			Assert.IsFalse(string25 == string30, "string25 & string30 should be different strings");
		}

	/// <summary>
	/// Test that Nullable&lt;&gt; wrappers around structs are returning the
	/// correct NH IType.
	/// </summary>
		[Test]
		public void GetNullableGeneric()
		{
			IType int64Type = NHibernateUtil.Int64;

			//Assert.AreEqual(int64Type, TypeFactory.HeuristicType("Int64?"), "'Int64?' should return a NH Int64Type");

			System.Type reflectedType = Util.ReflectHelper.ReflectedPropertyClass( typeof(GenericPropertyClass), "GenericInt64", "property" );
			Assert.AreEqual( int64Type, TypeFactory.HeuristicType( reflectedType ), "using System.Type should return nh Int64Type" );
			Assert.AreEqual( int64Type, TypeFactory.HeuristicType( reflectedType.AssemblyQualifiedName ), "using AQN should return nh Int64Type" );
			Assert.AreEqual( int64Type, TypeFactory.HeuristicType( reflectedType.FullName ), "using FullName should return nh Int64Type" );
		}

		public class GenericPropertyClass
		{
			private long? _genericLong;

			public long? GenericInt64
			{
				get { return _genericLong; }
				set { _genericLong = value; }
			}
		}

		private readonly Random rnd = new Random();

		[Test]
		public void MultiThreadAccess()
		{
			// Test added for NH-1251
			var mtr = new MultiThreadRunner<object>(
				100,
				o => TypeFactory.GetStringType(rnd.Next(1, 50)),
				o => TypeFactory.GetBinaryType(rnd.Next(1, 50)),
				o => TypeFactory.GetSerializableType(rnd.Next(1, 50)),
				o => TypeFactory.GetTypeType(rnd.Next(1, 20)))
			{
				EndTimeout = 2000,
				TimeoutBetweenThreadStart = 2
			};
			var totalCalls = mtr.Run(null);
			log.DebugFormat("{0} calls", totalCalls);
			var errors = mtr.GetErrors();
			if (errors.Length > 0)
			{
				Assert.Fail("One or more thread failed, found {0} errors. First exception: {1}", errors.Length, errors[0]);
			}
		}

		[Test]
		public void HoldQualifiedTypes()
		{
			var decimalType = TypeFactory.Basic("Decimal(10,5)");
			var doubleType = TypeFactory.Basic("Double(10,5)");
			var singleType = TypeFactory.Basic("Single(10,5)");
			var currencyType = TypeFactory.Basic("Currency(10,5)");

			Assert.That(decimalType, Is.SameAs(TypeFactory.Basic("Decimal(10,5)")));
			Assert.That(decimalType, Is.Not.SameAs(doubleType));
			Assert.That(decimalType, Is.Not.SameAs(singleType));
			Assert.That(decimalType, Is.Not.SameAs(currencyType));
			Assert.That(decimalType, Is.Not.SameAs(TypeFactory.Basic("Decimal(11,5)")));

			Assert.That(doubleType, Is.SameAs(TypeFactory.Basic("Double(10,5)")));
			Assert.That(doubleType, Is.Not.SameAs(TypeFactory.Basic("Double(11,5)")));
			Assert.That(doubleType, Is.Not.SameAs(singleType));
			Assert.That(doubleType, Is.Not.SameAs(currencyType));

			Assert.That(singleType, Is.Not.SameAs(currencyType));

			Assert.That(currencyType, Is.SameAs(TypeFactory.Basic("Currency(10,5)")));
			Assert.That(currencyType, Is.Not.SameAs(TypeFactory.Basic("Currency(11,5)")));

			Assert.That(singleType, Is.SameAs(TypeFactory.Basic("Single(10,5)")));
			Assert.That(singleType, Is.Not.SameAs(TypeFactory.Basic("Single(11,5)")));
		}

		public enum MyEnum
		{
			One
		}

		[Test]
		public void WhenUseEnumThenReturnGenericEnumType()
		{
			var iType = TypeFactory.HeuristicType(typeof (MyEnum).AssemblyQualifiedName);
			Assert.That(iType, Is.TypeOf<EnumType<MyEnum>>());
		}

		[Test]
		public void WhenUseNullableEnumThenReturnGenericEnumType()
		{
			var iType = TypeFactory.HeuristicType(typeof(MyEnum?).AssemblyQualifiedName);
			Assert.That(iType, Is.TypeOf<EnumType<MyEnum>>());
		}
		
		[Test]
		public void WhenUseEnumTypeThenReturnGenericEnumType()
		{
			var iType = TypeFactory.HeuristicType(typeof (MyEnum));
			Assert.That(iType, Is.TypeOf<EnumType<MyEnum>>());
		}

		[Test]
		public void WhenUseNullableEnumTypeThenReturnGenericEnumType()
		{
			var iType = TypeFactory.HeuristicType(typeof(MyEnum?));
			Assert.That(iType, Is.TypeOf<EnumType<MyEnum>>());
		}
	}
}
