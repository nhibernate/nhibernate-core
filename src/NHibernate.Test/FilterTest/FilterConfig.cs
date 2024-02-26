using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.FilterTest
{
	[TestFixture]
	public class FilterConfig
	{
		private string mappingCfg = "NHibernate.Test.FilterTest.FilterMapping.hbm.xml";

		[Test]
		public void FilterDefinitionsLoadedCorrectly()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(mappingCfg, this.GetType().Assembly);
			Assert.AreEqual(2, cfg.FilterDefinitions.Count);

			Assert.IsTrue(cfg.FilterDefinitions.ContainsKey("LiveFilter"));

			FilterDefinition f = cfg.FilterDefinitions["LiveFilter"];

			Assert.AreEqual(1, f.ParameterTypes.Count);

			BooleanType t = f.ParameterTypes["LiveParam"] as BooleanType;

			Assert.IsNotNull(t); //ensure that the parameter is the correct type. 
		}

		[Test]
		public void FiltersLoaded()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(mappingCfg, this.GetType().Assembly);

			ISessionFactory factory = cfg.BuildSessionFactory();

			ISession session = factory.OpenSession();

			IFilter filter = session.EnableFilter("LiveFilter");

			Assert.AreEqual("LiveFilter", filter.FilterDefinition.FilterName);

			filter.SetParameter("LiveParam", true);

			filter.Validate(); // make sure that everything is set up right. 

			IFilter filter2 = session.EnableFilter("LiveFilter2");

			filter2.SetParameter("LiveParam2", "somename");

			filter2.Validate();
		}

		[Test]
		public void TestFilterThrowsWithNoParameterSet()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(mappingCfg, this.GetType().Assembly);

			ISessionFactory factory = cfg.BuildSessionFactory();

			ISession session = factory.OpenSession();
			IFilter filter = session.EnableFilter("LiveFilter");
			Assert.Throws<HibernateException>(() => filter.Validate());
		}
	}
}
