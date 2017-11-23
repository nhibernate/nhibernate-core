using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Action;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.BulkManipulation
{
	[TestFixture]
	public class BulkOperationCleanupActionFixture
	{
		private ISessionImplementor _session;
		private ISessionFactoryImplementor _factory;
		private IEntityPersister _persister;

		[SetUp]
		public void SetupTest()
		{
			_session = Substitute.For<ISessionImplementor>();
			_factory = Substitute.For<ISessionFactoryImplementor>();
			_persister = Substitute.For<IEntityPersister>();
			_session.Factory.Returns(_factory);
			_factory.GetAllClassMetadata().Returns(new Dictionary<string, IClassMetadata> { ["TestClass"] = null });
			_factory.GetEntityPersister("TestClass").Returns(_persister);
			_factory.GetCollectionRolesByEntityParticipant("TestClass").Returns(new HashSet<string>(new[] { "TestClass.Children" }));
			_persister.QuerySpaces.Returns(new[] { "TestClass" });
			_persister.EntityName.Returns("TestClass");
		}

		[TestCase("TestClass", true, 1, 1, 1)]
		[TestCase("AnotherClass", true, 1, 0, 0)]
		[TestCase("AnotherClass,TestClass", true, 2, 1, 1)]
		[TestCase("TestClass", false, 1, 0, 1)]
		[TestCase("", true, 1, 1, 1)]
		[Test]
		public void AfterTransactionCompletionProcess_EvictsFromCache(string querySpaces, bool persisterHasCache, int expectedPropertySpaceLength, int expectedEntityEvictionCount, int expectedCollectionEvictionCount)
		{
			_persister.HasCache.Returns(persisterHasCache);

			var target = new BulkOperationCleanupAction(_session, new HashSet<string>(querySpaces.Split(new []{','},StringSplitOptions.RemoveEmptyEntries)));

			target.AfterTransactionCompletionProcess(true);

			Assert.AreEqual(expectedPropertySpaceLength, target.PropertySpaces.Length);

			if (expectedEntityEvictionCount > 0)
			{
				_factory.Received(1).EvictEntity(Arg.Is<IEnumerable<string>>(x => x.Count() == expectedEntityEvictionCount));
			}
			else
			{
				_factory.DidNotReceive().EvictEntity(Arg.Any<IEnumerable<string>>());
			}

			if (expectedCollectionEvictionCount > 0)
			{
				_factory.Received(1).EvictCollection(Arg.Is<IEnumerable<string>>(x => x.Count() == expectedCollectionEvictionCount));
			}
			else
			{
				_factory.DidNotReceive().EvictCollection(Arg.Any<IEnumerable<string>>());
			}

		}

		[TestCase("TestClass", true, 1, 1, 1)]
		[TestCase("AnotherClass", true, 1, 0, 0)]
		[TestCase("AnotherClass,TestClass", true, 2, 1, 1)]
		[TestCase("TestClass", false, 1, 0, 1)]
		[TestCase("", true, 1, 1, 1)]
		[Test]
		public async Task InitAsync_EvictsFromCacheAsync(string querySpaces, bool persisterHasCache, int expectedPropertySpaceLength, int expectedEntityEvictionCount, int expectedCollectionEvictionCount)
		{
			
			_persister.HasCache.Returns(persisterHasCache);

			var target = new BulkOperationCleanupAction(_session, new HashSet<string>(querySpaces.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));

			await target.InitAsync(CancellationToken.None);

			Assert.AreEqual(expectedPropertySpaceLength, target.PropertySpaces.Length);

			if (expectedEntityEvictionCount > 0)
			{
				await _factory.Received(1).EvictEntityAsync(Arg.Is<IEnumerable<string>>(x => x.Count() == expectedEntityEvictionCount));
			}
			else
			{
				await _factory.DidNotReceive().EvictEntityAsync(Arg.Any<IEnumerable<string>>());
			}

			if (expectedCollectionEvictionCount > 0)
			{
				await _factory.Received(1).EvictCollectionAsync(Arg.Is<IEnumerable<string>>(x => x.Count() == expectedCollectionEvictionCount));
			}
			else
			{
				await _factory.DidNotReceive().EvictCollectionAsync(Arg.Any<IEnumerable<string>>());
			}

		}
	}
}
