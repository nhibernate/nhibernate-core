using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.MappingExceptions
{
	//NH-674
	[TestFixture]
	public class DuplicateMappingFixture
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
				Assert.AreEqual(
					"Could not compile the mapping document: NHibernate.Test.MappingExceptions.DuplicateClassMapping.hbm.xml",
					me.Message);
				Assert.IsTrue(me.InnerException.GetType() == typeof(DuplicateMappingException));
				Assert.AreEqual("Duplicate class/entity mapping NHibernate.Test.MappingExceptions.A", me.InnerException.Message);
			}
		}

		[Test]
		public void MappingSameCollectionTwiceShouldThrow()
		{
			Configuration cfg = new Configuration();
			string resource = "NHibernate.Test.MappingExceptions.DuplicateCollectionMapping.hbm.xml";
			try
			{
				cfg.AddResource(resource, this.GetType().Assembly);
				cfg.BuildSessionFactory();
				Assert.Fail("Should have thrown exception when we mapped the same class twice");
			}
			catch (MappingException me)
			{
				Assert.AreEqual(
					"Could not compile the mapping document: NHibernate.Test.MappingExceptions.DuplicateCollectionMapping.hbm.xml",
					me.Message);
				Assert.IsTrue(me.InnerException.GetType() == typeof(DuplicateMappingException));
				Assert.AreEqual("Duplicate collection role mapping NHibernate.Test.MappingExceptions.A.Children",
				                me.InnerException.Message);
			}
		}
	}
}