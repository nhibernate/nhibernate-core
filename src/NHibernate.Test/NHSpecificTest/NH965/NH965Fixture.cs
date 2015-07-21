using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH965
{
    [TestFixture]
    public class NH965Fixture
    {
        [Test]
        public void Bug()
        {
            Configuration cfg = new Configuration();
            cfg.AddResource(GetType().Namespace + ".Mappings.hbm.xml", GetType().Assembly);
            cfg.BuildSessionFactory().Close();
        }
    }
}
