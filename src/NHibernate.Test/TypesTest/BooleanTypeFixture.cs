using System;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the BooleanType.
	/// </summary>
	[TestFixture]
	public class BooleanTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Boolean"; }
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals()
		{
			BooleanType type = (BooleanType) NHibernateUtil.Boolean;

			Assert.IsTrue(type.Equals(true, true));
			Assert.IsTrue(type.Equals(false, false));
			Assert.IsFalse(type.Equals(true, false));
		}

		[Test]
		public void ReadWrite()
		{
			BooleanClass basic = new BooleanClass();
			basic.Id = 1;
			basic.BooleanValue = true;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (BooleanClass) s.Load(typeof(BooleanClass), 1);

			Assert.AreEqual(true, basic.BooleanValue);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}
	}
}