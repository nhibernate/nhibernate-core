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
		[Test]
		public void InvalidGenericMapping()
		{
			Assert.Throws<MappingException>(
				() =>
				new Configuration().AddResource(typeof (Fixture).Namespace + ".Mapping.hbm.xml", typeof (Fixture).Assembly).
					BuildSessionFactory());
		}
	}
}
