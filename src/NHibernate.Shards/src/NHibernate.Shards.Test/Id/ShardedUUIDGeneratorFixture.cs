using NHibernate.Shards.Id;
using NHibernate.Shards.Session;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Id
{
	[TestFixture]
	public class ShardedUUIDGeneratorFixture
	{
		[Test]
		public void GenerateShardedUUID()
		{
			string Id;
			ShardedUUIDGenerator generator = new ShardedUUIDGenerator();

			ShardedSessionImpl.CurrentSubgraphShardId = new ShardId(13);
			Id = (string) generator.Generate(null, null);
			Assert.AreEqual(true, Id.StartsWith("000d"));

			ShardedSessionImpl.CurrentSubgraphShardId = new ShardId(32767); //short MaxValue
			Id = (string) generator.Generate(null, null);
			Assert.AreEqual(true, Id.StartsWith("7fff"));
		}

		[Test]
		public void GetEncodedShardId()
		{
			string Id;
			ShardedUUIDGenerator generator = new ShardedUUIDGenerator();

			ShardedSessionImpl.CurrentSubgraphShardId = new ShardId(25);
			Id = (string)generator.Generate(null, null);
			Assert.AreEqual(new ShardId(25), generator.ExtractShardId(Id));

			ShardedSessionImpl.CurrentSubgraphShardId = new ShardId(599);;
			Id = (string)generator.Generate(null, null);
			Assert.AreEqual(new ShardId(599), generator.ExtractShardId(Id));

			ShardedSessionImpl.CurrentSubgraphShardId = new ShardId(short.MaxValue); ;
			Id = (string)generator.Generate(null, null);
			Assert.AreEqual(new ShardId(short.MaxValue), generator.ExtractShardId(Id));
		}
	}
}