using NHibernate.Collection;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1515
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void IsInitializedCallsWasInitializedFromLazyCollection()
		{
			var collection = Substitute.For<ILazyInitializedCollection>();

			NHibernateUtil.IsInitialized(collection);

			var assertRes = collection.Received().WasInitialized;
		}

		[Test]
		public void IntializeForwaredToLazyCollection()
		{
			var collection = Substitute.For<ILazyInitializedCollection>();

			NHibernateUtil.Initialize(collection);

			collection.Received().ForceInitialization();
		}

		[Test]
		public void IsInitializedCallsWasInitializedFromPersistentCollection()
		{
			var collection = Substitute.For<IPersistentCollection>();

			NHibernateUtil.IsInitialized(collection);

			var assertRes = collection.Received().WasInitialized;
		}

		[Test]
		public void IntializeForwaredToPersistentCollection()
		{
			var collection = Substitute.For<IPersistentCollection>();

			NHibernateUtil.Initialize(collection);

			collection.Received().ForceInitialization();
		}
	}
}
