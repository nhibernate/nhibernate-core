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
		public void OrderingAssemblyOrderer()
		{
			AssemblyHbmOrderer orderer = AssemblyHbmOrderer.CreateWithResources(MyAssembly, Resources);
			IList<string> orderedResources = orderer.GetHbmFiles();
			Assert.AreEqual(Resources.Length, orderedResources.Count);
			Assert.AreEqual(Resources[3], orderedResources[0]);
			Assert.AreEqual(Resources[1], orderedResources[1]);
			Assert.AreEqual(Resources[2], orderedResources[2]);
			Assert.AreEqual(Resources[0], orderedResources[3]);
		}

		[Test]
		public void OrderingAddResources()
		{
			Configuration cfg = new Configuration();
			cfg.AddResources(MyAssembly, Resources, false);
			cfg.BuildSessionFactory().Close();
		}
	}
}
