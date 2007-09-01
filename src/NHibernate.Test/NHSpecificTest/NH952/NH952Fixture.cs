using System;
using System.Collections;
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
			IList orderedResources = orderer.GetHbmFiles();
			Assert.AreEqual(Resources.Length, orderedResources.Count);
			Assert.AreEqual(Resources[3], ((EmbeddedResource) orderedResources[0]).Name);
			Assert.AreEqual(Resources[1], ((EmbeddedResource) orderedResources[1]).Name);
			Assert.AreEqual(Resources[2], ((EmbeddedResource) orderedResources[2]).Name);
			Assert.AreEqual(Resources[0], ((EmbeddedResource) orderedResources[3]).Name);
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
