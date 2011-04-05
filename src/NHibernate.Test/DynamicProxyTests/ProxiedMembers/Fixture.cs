using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace NHibernate.Test.DynamicProxyTests.ProxiedMembers
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "DynamicProxyTests.ProxiedMembers.Mapping.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
        [Ignore]
		public void Proxy()
		{
			ISession s = OpenSession();
			ClassWithVarietyOfMembers c = new ClassWithVarietyOfMembers {Id = 1, Data = "some data"};
			s.Save(c);
			s.Flush();
			s.Close();

			s = OpenSession();
            c = (ClassWithVarietyOfMembers)s.Load(typeof(ClassWithVarietyOfMembers), c.Id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(c));

		    int x;
		    c.Method1(out x);
		    Assert.AreEqual(3, x);

            x = 4;
            c.Method2(ref x);
            Assert.AreEqual(5, x);

			s.Delete(c);
			s.Flush();
			s.Close();
		}
	}
}