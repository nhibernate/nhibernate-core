using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// Represents a replication strategy
	/// </summary>
	public abstract class ReplicationMode
	{
		private readonly int code;
		private readonly string name;
		private static readonly IDictionary Instances = new Hashtable();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="name"></param>
		public ReplicationMode(int level, string name)
		{
			this.code = level;
			this.name = name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="currentVersion"></param>
		/// <param name="newVersion"></param>
		/// <param name="versionType"></param>
		/// <returns></returns>
		public abstract bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
		                                                   IVersionType versionType);

		/// <summary></summary>
		public static readonly ReplicationMode Exception = new ExceptionReplicationMode(0, "EXCEPTION");

		private sealed class ExceptionReplicationMode : ReplicationMode
		{
			public ExceptionReplicationMode(int level, string name) : base(level, name)
			{
			}

			/// <summary>
			/// Throw an exception when a row already exists
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="currentVersion"></param>
			/// <param name="newVersion"></param>
			/// <param name="versionType"></param>
			/// <returns></returns>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				throw new NotSupportedException("should not be called");
			}
		}

		/// <summary></summary>
		public static readonly ReplicationMode Ignore = new IgnoreReplicationMode(1, "IGNORE");

		private sealed class IgnoreReplicationMode : ReplicationMode
		{
			public IgnoreReplicationMode(int level, string name) : base(level, name)
			{
			}

			/// <summary>
			/// Ignore replicated entities when a row already exists
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="currentVersion"></param>
			/// <param name="newVersion"></param>
			/// <param name="versionType"></param>
			/// <returns></returns>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				return false;
			}
		}

		/// <summary></summary>
		public static readonly ReplicationMode Overwrite = new OverwriteReplicationMode(3, "OVERWRITE");

		private sealed class OverwriteReplicationMode : ReplicationMode
		{
			public OverwriteReplicationMode(int level, string name) : base(level, name)
			{
			}

			/// <summary>
			/// Overwrite existing rows when a row already exists
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="currentVersion"></param>
			/// <param name="newVersion"></param>
			/// <param name="versionType"></param>
			/// <returns></returns>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				return true;
			}
		}

		/// <summary></summary>
		public static readonly ReplicationMode LatestVersion = new LatestVersionReplicationMode(2, "LATEST_VERSION");

		private sealed class LatestVersionReplicationMode : ReplicationMode
		{
			public LatestVersionReplicationMode(int level, string name) : base(level, name)
			{
			}

			/// <summary>
			/// When a row already exists, choose the latest version
			/// </summary>
			/// <param name="entity"></param>
			/// <param name="currentVersion"></param>
			/// <param name="newVersion"></param>
			/// <param name="versionType"></param>
			/// <returns></returns>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				if (versionType == null)
				{
					// always overwrite nonversioned data
					return true;
				}

				return versionType.Comparator.Compare(currentVersion, newVersion) <= 0;
			}
		}
	}
}