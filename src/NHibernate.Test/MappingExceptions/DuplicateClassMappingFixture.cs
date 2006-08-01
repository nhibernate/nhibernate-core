using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
    [TestFixture]
    public class DuplicateClassMappingFixture
    {
        [Test]
        public void MappingTheSameClassTwiceShouldThrowException()
        {
            Configuration cfg = new Configuration();
            string resource = "NHibernate.Test.MappingExceptions.DuplicateClassMapping.hbm.xml";
            try
            {
                cfg.AddResource(resource, this.GetType().Assembly);
                cfg.BuildSessionFactory();
                Assert.Fail("Should have thrown exception when we mapped the same class twice");
            }
            catch (MappingException me)
            {
                Assert.AreEqual("Could not compile the mapping document: NHibernate.Test.MappingExceptions.DuplicateClassMapping.hbm.xml", me.Message);
                Assert.AreEqual("duplicate class mapping: A", me.InnerException.Message);
            }
        }
    }
}
