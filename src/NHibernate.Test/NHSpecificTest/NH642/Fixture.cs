using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH642
{
	public class MissingSetter
	{
		public virtual int ReadOnly
		{
			get { return 0; }
		}
	}

	public class MissingGetter
	{
		public virtual int WriteOnly
		{
			set { }
		}
	}

	[TestFixture]
	public class Fixture
	{
		private void DoTest(string name)
		{
			ISessionFactory factory = new Configuration()
				.AddResource("NHibernate.Test.NHSpecificTest.NH642." + name + ".hbm.xml",
				             typeof(Fixture).Assembly)
				.BuildSessionFactory();
			factory.Close();
		}

		[Test, ExpectedException(typeof(PropertyNotFoundException))]
		public void MissingGetter()
		{
			DoTest("MissingGetter");
		}

		[Test, ExpectedException(typeof(PropertyNotFoundException))]
		public void MissingSetter()
		{
			DoTest("MissingSetter");
		}
	}
}