using System;

using NHibernate.Type;
using NHibernate.Test.TypesTest;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Tests for mapping a Double Property to a database field.
	/// </summary>
	[TestFixture]
	public class DoubleTypeFixture : TestCase 
	{
		double[] _values = new double[2];

		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "TypesTest.DoubleClass.hbm.xml"}, true, "NHibernate.Test" );
		}

		[SetUp]
		public void SetUp() 
		{
			if( dialect is Dialect.OracleDialect ) 
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

		[TearDown]
		public override void TearDown() 
		{
			// do nothing except not let the base TearDown get called
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			base.TearDown();
		}

		#endregion

		/// <summary>
		/// Verify Equals will correctly determine when the property
		/// is dirty.
		/// </summary>
		[Test]
		public void Equals() 
		{
			DoubleType type = (DoubleType)NHibernateUtil.Double;

			Assert.IsTrue( type.Equals( 1.5e20, 1.5e20 ) );
			Assert.IsFalse( type.Equals( 1.5e20, 1.4e20 ) );
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
			basic = (DoubleClass)s.Load( typeof(DoubleClass), 1 );

			Assert.AreEqual( _values[0], basic.DoubleValue );

			s.Delete( basic );
			s.Flush();
			s.Close();
		}
	}
}

