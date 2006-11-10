#if NET_2_0
using System;
using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH606
{
	public class HasNonGenericList
	{
		private int id;
		private IList nonGenericList;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual IList NonGenericList
		{
			get { return nonGenericList; }
			set { nonGenericList = value; }
		}
	}

	[TestFixture]
	public class Fixture
	{
		[Test, ExpectedException(typeof(MappingException))]
		public void InvalidGenericMapping()
		{
			ISessionFactory sf = new Configuration()
				.AddResource(typeof (Fixture).Namespace + ".Mapping.hbm.xml", typeof (Fixture).Assembly)
				.BuildSessionFactory();
			sf.Close();
		}
	}
}
#endif