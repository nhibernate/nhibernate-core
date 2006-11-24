using System;
using System.Collections;
using System.Reflection;
using System.Text;
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

			Assert.IsFalse(string25==string30, "string25 & string30 should be different strings");
		}
		
#if NET_2_0
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
#endif
	}
}
