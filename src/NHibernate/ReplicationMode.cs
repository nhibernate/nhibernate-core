using NHibernate.Type;

namespace NHibernate
{
	/// <summary> 
	/// Represents a replication strategy. 
	/// </summary>
	/// <seealso cref="ISession.Replicate(object, ReplicationMode)"/>
	public abstract class ReplicationMode
	{
		public static readonly ReplicationMode Exception = new ExceptionReplicationMode("EXCEPTION");
		public static readonly ReplicationMode Ignore = new IgnoreReplicationMode("IGNORE");
		public static readonly ReplicationMode LatestVersion = new LatestVersionReplicationMode("LATEST_VERSION");
		public static readonly ReplicationMode Overwrite = new OverwriteReplicationMode("OVERWRITE");
		private readonly string name;

		protected ReplicationMode(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}

		public abstract bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
		                                                   IVersionType versionType);

		#region Nested type: ExceptionReplicationMode

		private sealed class ExceptionReplicationMode : ReplicationMode
		{
			public ExceptionReplicationMode(string name) : base(name) {}

			/// <summary>
			/// Throw an exception when a row already exists
			/// </summary>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				throw new AssertionFailure("should not be called");
			}
		}

		#endregion

		#region Nested type: IgnoreReplicationMode

		private sealed class IgnoreReplicationMode : ReplicationMode
		{
			public IgnoreReplicationMode(string name) : base(name) {}

			/// <summary>
			/// Ignore replicated entities when a row already exists
			/// </summary>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				return false;
			}
		}

		#endregion

		#region Nested type: LatestVersionReplicationMode

		private sealed class LatestVersionReplicationMode : ReplicationMode
		{
			public LatestVersionReplicationMode(string name) : base(name) {}

			/// <summary>
			/// When a row already exists, choose the latest version
			/// </summary>
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

		#endregion

		#region Nested type: OverwriteReplicationMode

		private sealed class OverwriteReplicationMode : ReplicationMode
		{
			public OverwriteReplicationMode(string name) : base(name) {}

			/// <summary>
			/// Overwrite existing rows when a row already exists
			/// </summary>
			public override bool ShouldOverwriteCurrentVersion(object entity, object currentVersion, object newVersion,
			                                                   IVersionType versionType)
			{
				return true;
			}
		}

		#endregion
	}
}