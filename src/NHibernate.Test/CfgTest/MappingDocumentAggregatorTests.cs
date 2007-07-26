using System.Collections.Generic;
using System.Reflection;

using NHibernate.Cfg.MappingSchema;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class MappingDocumentAggregatorTests
	{
		[Test]
		public void CanAddDomainModelAssembly()
		{
			Assembly domainModelAssembly = typeof (DomainModel.A).Assembly;

			MappingDocumentAggregator aggregator = new MappingDocumentAggregator();
			aggregator.Add(domainModelAssembly);
			IList<HbmMapping> results = aggregator.List();
			Assert.IsTrue(results.Count > 0); // 54
		}
	}
}