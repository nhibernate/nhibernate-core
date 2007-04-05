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
			AssemblyHbmOrderer orderer = new AssemblyHbmOrderer(domain);
			orderer.AddResource("NHibernate.DomainModel.MultiExtends.hbm.xml");
			orderer.AddResource("NHibernate.DomainModel.Multi.hbm.xml");
			orderer.AddResource("NHibernate.DomainModel.Query.hbm.xml");

			IList files = orderer.GetHbmFiles();
			Assert.IsTrue(files.Contains("NHibernate.DomainModel.Query.hbm.xml"));
		}
	}
}