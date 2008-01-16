using System;
using System.Globalization;
using log4net;
using NHibernate.Id;
using NHibernate.Shards.Session;
using NHibernate.Shards.Util;

namespace NHibernate.Shards.Id
{
	/// <summary>
	/// TODO: documentation
	/// TODO: See if ShardedUUIDGenerator need to inherit from UUIDHexGenerator
	/// </summary>
	public class ShardedUUIDGenerator : UUIDHexGenerator, IShardEncodingIdentifierGenerator
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ShardedUUIDGenerator));

		public ShardId ExtractShardId(object identifier)
		{
			Preconditions.CheckNotNull(identifier);

			string Id = (string) identifier;

			int shardId = int.Parse(Id.Substring(0, 4), NumberStyles.AllowHexSpecifier);

			return new ShardId(shardId);
		}

		protected override string GenerateNewGuid()
		{
			byte[] guidArray = Guid.NewGuid().ToByteArray();

			//Make clean the space for the ShardId: 0000xxxx...xxxx
			Array.Copy(new byte[] {0, 0}, guidArray, 2);

			byte[] shardIdArray = BitConverter.GetBytes(GetShardId());

			Array.Copy(shardIdArray, 0, guidArray, shardIdArray.Length, shardIdArray.Length);

			return new Guid(guidArray).ToString(format);
		}

		private short GetShardId()
		{
			ShardId shardId = ShardedSessionImpl.CurrentSubgraphShardId;
			Preconditions.CheckState(shardId != null);
			return Convert.ToInt16(shardId.Id);
		}
	}
}