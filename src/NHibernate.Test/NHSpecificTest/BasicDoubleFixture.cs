
using System;

using NHibernate.DomainModel.NHSpecific;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Tests for mapping a Double Property to a database field.
	/// </summary>
	[TestFixture]
	public class BasicDoubleFixture : TestCase 
	{
		double[] _values = new double[2];

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.BasicDouble.hbm.xml"}, true );
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

		[Test]
		public void Insert() 
		{
			BasicDouble basic = Create(1);

			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();

			BasicDouble basicLoaded = (BasicDouble)s.Load(typeof(BasicDouble), 1);

			Assert.IsNotNull(basicLoaded);
			Assert.IsFalse(basic==basicLoaded);

			Assert.AreEqual( basic.DoubleValue, basicLoaded.DoubleValue );

			s.Delete(basicLoaded);
			s.Flush();
			s.Close();
		}

		[Test]
		public void Update() 
		{
			BasicDouble basic = Create(1);
			
			ISession s = sessions.OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			basic = (BasicDouble)s.Load(typeof(BasicDouble), 1);

			basic.DoubleValue = _values[1];

			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			// make sure the update went through
			BasicDouble basicLoaded = (BasicDouble)s.Load(typeof(BasicDouble), 1);

			Assert.AreEqual( basic.DoubleValue, basicLoaded.DoubleValue );
			
			s.Delete(basicLoaded);
			s.Flush();
			s.Close();
		}

		private BasicDouble Create(int id) 
		{
			BasicDouble basic = new BasicDouble();
			basic.Id = id;

			basic.DoubleValue = _values[0];

			return basic;
		}

	}
}

