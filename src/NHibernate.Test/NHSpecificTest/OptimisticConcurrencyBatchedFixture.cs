using System.Collections;
using System.Collections.Generic;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class OptimisticConcurrencyBatchedFixture : OptimisticConcurrencyFixture
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchStrategy, typeof(GenericBatchingBatcherFactory).AssemblyQualifiedName);
			configuration.SetProperty(Environment.BatchSize, "5");
			configuration.SetProperty(Environment.BatchVersionedData, "true");
		}
	}
}
