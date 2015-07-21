using System.Collections.Generic;
using System.Reflection;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH952
{
	[TestFixture]
	public class NH952Fixture
	{
		private static readonly Assembly MyAssembly = typeof(NH952Fixture).Assembly;
		private static readonly string MyNamespace = typeof(NH952Fixture).Namespace;
		
		private static readonly string[] Resources = new string[]
			{
				// Order is important!
				MyNamespace + ".Asset.hbm.xml",
				MyNamespace + ".SellableItem.hbm.xml",
				MyNamespace + ".PhysicalItem.hbm.xml",
				MyNamespace + ".Item.hbm.xml"
			};

		[Test]
		public void OrderingAddResources()
		{
			Configuration cfg = new Configuration();
			foreach (string res in Resources)
			{
				cfg.AddResource(res, MyAssembly);
			}
			cfg.BuildSessionFactory().Close();
		}
	}
}
