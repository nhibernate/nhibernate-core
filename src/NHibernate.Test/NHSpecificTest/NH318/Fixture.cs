using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH318
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH318.Mappings.hbm.xml"}; }
		}

		[Test]
		public void DeleteWithNotNullPropertySetToNull()
		{
			ISession session = OpenSession();
			NotNullPropertyHolder a = new NotNullPropertyHolder();
			a.NotNullProperty = "Value";
			session.Save(a);
			session.Flush();
			a.NotNullProperty = null;
			session.Delete(a);
			session.Flush();
			session.Close();
		}
	}
}