using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1515
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		[Test]
		public void LazyCollectionCanBeInitialized()
		{
			var collection = new LayzInitializationTestCollection(false);
			Assert.That(!NHibernateUtil.IsInitialized(collection));
			NHibernateUtil.Initialize(collection);
			Assert.That(NHibernateUtil.IsInitialized(collection));
		}

		[Test]
		public void PersistentCollectionCanBeInitialized()
		{
			var collection = new PersistentLayzInitializationTestCollection(false);
			Assert.That(!NHibernateUtil.IsInitialized(collection));
			NHibernateUtil.Initialize(collection);
			Assert.That(NHibernateUtil.IsInitialized(collection));
		}

	}
}
