using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest 
{

	public enum A 
	{
		Zero,
		One,
		Two
	}

	public enum B 
	{
		Zero,
		One,
		Two
	}

	/// <summary>
	/// The Unit Test for the PersistentEnum Type.
	/// </summary>
	[TestFixture]
	public class PersistentEnumTypeFixture : BaseTypeFixture
	{	
		[Test]
		public void EqualsTrue() 
		{
			IType type = NHibernate.Enum(typeof(A));

			A lhs = A.One;
			A rhs = A.One;

			Assert.IsTrue( type.Equals(lhs, rhs) );
		}

		/// <summary>
		/// Verify that even if the Enum have the same underlying value but they
		/// are different Enums that they are not considered Equal.
		/// </summary>
		[Test]
		public void EqualsFalseSameUnderlyingValue() 
		{
			IType type = NHibernate.Enum(typeof(A));

			A lhs = A.One;
			B rhs = B.One;

			Assert.IsFalse(type.Equals(lhs, rhs));
		}

		[Test]
		public void EqualsFalse() 
		{
			IType type = NHibernate.Enum(typeof(A));

			A lhs = A.One;
			A rhs = A.Two;

			Assert.IsFalse(type.Equals(lhs, rhs));
		}

	}


}
