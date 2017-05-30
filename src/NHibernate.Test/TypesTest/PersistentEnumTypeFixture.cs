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
			p = new PersistentEnumClass(1, A.One, B.Two);
		}


		[Test]
		public void EqualsTrue()
		{
			IType type = NHibernateUtil.Enum(typeof(A));

			A lhs = A.One;
			A rhs = A.One;

			Assert.IsTrue(type.IsEqual(lhs, rhs));
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

			Assert.IsFalse(type.IsEqual(lhs, rhs));
		}

		[Test]
		public void EqualsFalse()
		{
			IType type = NHibernateUtil.Enum(typeof(A));

			A lhs = A.One;
			A rhs = A.Two;

			Assert.IsFalse(type.IsEqual(lhs, rhs));
		}

		[Test]
		public void UsageInHqlSelectNew()
		{
			using (ISession s = OpenSession())
			{
				s.Save(p);
				s.Flush();
			}

			using (ISession s = Sfi.OpenSession())
			{
				s.CreateQuery("select new PersistentEnumHolder(p.A, p.B) from PersistentEnumClass p").List();
				s.Delete("from PersistentEnumClass");
				s.Flush();
			}
		}

		[Test]
		public void UsageInHqlSelectNewInvalidConstructor()
		{
			using (ISession s = OpenSession())
			{
				s.Save(p);
				s.Flush();
			}

			ISession s2 = Sfi.OpenSession();
			try
			{
				Assert.Throws<QueryException>(
					() => s2.CreateQuery("select new PersistentEnumHolder(p.id, p.A, p.B) from PersistentEnumClass p").List());
			}
			finally
			{
				s2.Delete("from PersistentEnumClass");
				s2.Flush();
				s2.Close();
			}
		}

		[Test]
		public void CanWriteAndReadUsingBothHeuristicAndExplicitGenericDeclaration()
		{
			var persistentEnumClass = new PersistentEnumClass {Id = 1, A = A.Two, B = B.One};
			using (ISession s = OpenSession())
			{
				s.Save(persistentEnumClass);
				s.Flush();
			}

			using (ISession s = Sfi.OpenSession())
			{
				var saved = s.Get<PersistentEnumClass>(1);
				Assert.That(saved.A, Is.EqualTo(A.Two));
				Assert.That(saved.B, Is.EqualTo(B.One));
				s.Delete(saved);
				s.Flush();
			}
		}
	}

	public class GenericEnumTypeTest
	{
		[Test]
		public void TheNameShouldBeFullNameAndAssembly()
		{
			var enumType = new EnumType<B>();
			Assert.That(enumType.Name, Is.EqualTo(typeof(EnumType<B>).FullName + ", NHibernate"));
		}
	}
}