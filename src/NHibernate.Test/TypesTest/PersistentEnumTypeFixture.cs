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
	public class PersistentEnumTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "PersistentEnum"; }
		}

		private PersistentEnumClass p;

		protected override void OnSetUp()
		{
			base.OnSetUp();
			p = new PersistentEnumClass();
			p.Id = 1;
			p.A = A.One;
			p.B = B.Two;
		}


		[Test]
		public void EqualsTrue() 
		{
			IType type = NHibernateUtil.Enum(typeof(A));

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
			IType type = NHibernateUtil.Enum(typeof(A));

			A lhs = A.One;
			B rhs = B.One;

			Assert.IsFalse(type.Equals(lhs, rhs));
		}

		[Test]
		public void EqualsFalse() 
		{
			IType type = NHibernateUtil.Enum(typeof(A));

			A lhs = A.One;
			A rhs = A.Two;

			Assert.IsFalse(type.Equals(lhs, rhs));
		}

		[Test]
		public void UsageInHqlSelectNew()
		{
			using( ISession s = OpenSession() )
			{
				s.Save( p );
				s.Flush();
			}

			using (ISession s = sessions.OpenSession())
			{
				s.Find( "select new PersistentEnumClass(p.A, p.B) from PersistentEnumClass p");
				s.Delete( "from PersistentEnumClass" );
				s.Flush();
			}
		}

		[Test, ExpectedException(typeof(QueryException))]
		public void UsageInHqlSelectNewInvalidConstructor()
		{
			using( ISession s = OpenSession() )
			{
				s.Save( p );
				s.Flush();
			}

			ISession s2 = sessions.OpenSession();
			try
			{
				s2.Find( "select new PersistentEnumClass(p.A) from PersistentEnumClass p");
			}
			finally
			{
				s2.Delete( "from PersistentEnumClass" );
				s2.Flush();
				s2.Close();
			}
		}
	}


}
