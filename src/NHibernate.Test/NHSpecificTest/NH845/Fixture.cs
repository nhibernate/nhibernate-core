using System;
using System.Collections;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH845
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void HbmOrdererForgetsMappingFilesWithoutClassesIfExtendsIsUsed()
		{
			Assembly domain = typeof(Master).Assembly;
			AssemblyHbmOrderer orderer = new AssemblyHbmOrderer();
			orderer.AddResource(domain, "NHibernate.DomainModel.MultiExtends.hbm.xml");
			orderer.AddResource(domain, "NHibernate.DomainModel.Multi.hbm.xml");
			orderer.AddResource(domain, "NHibernate.DomainModel.Query.hbm.xml");

			IList files = orderer.GetOrderedResources();
			Assert.IsTrue(files.Contains(new EmbeddedResource(domain, "NHibernate.DomainModel.Query.hbm.xml")));
		}
	}
}