using System;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Tests for mapping a Double Property to a database field.
	/// </summary>
	[TestFixture]
	public class DoubleTypeFixture : TypeFixtureBase
	{
		private double[] _values = new double[2];

		protected override string TypeName
		{
			get { return "Double"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			if (Dialect is Oracle8iDialect)
			{
				_values[0] = 1.5e20;
				_values[1] = 1.2e-20;
			}
			else
			{
				_values[0] = 1.5e35;
				_values[1] = 1.2e-35;
			}
		}

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals()
		{
			DoubleType type = (DoubleType) NHibernateUtil.Double;

			Assert.IsTrue(type.IsEqual(1.5e20, 1.5e20));
			Assert.IsFalse(type.IsEqual(1.5e20, 1.4e20));
		}

		[Test]
		public void ReadWrite()
		{
			DoubleClass basic = new DoubleClass();
			basic.Id = 1;
			basic.DoubleValue = _values[0];

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (DoubleClass) s.Load(typeof(DoubleClass), 1);

			Assert.AreEqual(_values[0], basic.DoubleValue);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}
	}
}