using System;

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
			NullableType guidType = TypeFactory.GetGuidType();
			NullableType guidType2 = TypeFactory.GetGuidType();

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

		/// <summary>
		/// Test that the String returned from NHibernate.String and TypeFactory.GetStringType 
		/// returns the exact same Type.
		/// </summary>
		[Test]
		public void GetDefaultString() 
		{
			NullableType stringFromNH = NHibernateUtil.String;
			NullableType stringFromTF = TypeFactory.GetStringType();

			Assert.AreSame(stringFromNH, stringFromTF);
		}
	}
}
