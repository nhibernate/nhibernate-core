using System.Collections.Generic;
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
			Configuration cfg = new Configuration();
			Assembly domain = typeof(Master).Assembly;
			cfg.AddResource("NHibernate.DomainModel.MasterDetail.hbm.xml", domain);
			cfg.AddResource("NHibernate.DomainModel.MultiExtends.hbm.xml", domain);
			cfg.AddResource("NHibernate.DomainModel.Multi.hbm.xml", domain);
			cfg.AddResource("NHibernate.DomainModel.Query.hbm.xml", domain);

			ISessionFactory sf = cfg.BuildSessionFactory();

			try
			{
				using (ISession session = sf.OpenSession())
				{
					Assert.IsNotNull(session.GetNamedQuery("AQuery"));
				}
			}
			finally
			{
				sf.Close();
			}
		}
	}
}
