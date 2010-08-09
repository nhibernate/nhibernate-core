
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Utility methods for managing versions and timestamps
	/// </summary>
	public class Versioning
	{
		public enum OptimisticLock
		{
			None = -1,
			Version = 0,
			Dirty = 1,
			All = 2
		}

		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(Versioning));

		/// <summary>
		/// Increment the given version number
		/// </summary>
		/// <param name="version">The value of the current version.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <param name="session">The current <see cref="ISession" />.</param>
		/// <returns>Returns the next value for the version.</returns>
		public static object Increment(object version, IVersionType versionType, ISessionImplementor session)
		{
			object next = versionType.Next(version, session);
			if (log.IsDebugEnabled)
			{
				log.Debug(
					string.Format("Incrementing: {0} to {1}", versionType.ToLoggableString(version, session.Factory),
												versionType.ToLoggableString(next, session.Factory)));
			}
			return next;
		}

		/// <summary>
		/// Create an initial version number
		/// </summary>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <param name="session">The current <see cref="ISession" />.</param>
		/// <returns>A seed value to initialize the versioned property with.</returns>
		public static object Seed(IVersionType versionType, ISessionImplementor session)
		{
			object seed = versionType.Seed(session);
			if (log.IsDebugEnabled)
			{
				log.Debug("Seeding: " + seed);
			}
			return seed;
		}

		/// <summary>
		/// Seed the given instance state snapshot with an initial version number
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="versionProperty">The index of the version property in the <c>fields</c> parameter.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <param name="force">Force the version to initialize</param>
		/// <param name="session">The current session, if any.</param>
		/// <returns><see langword="true" /> if the version property needs to be seeded with an initial value.</returns>
		public static bool SeedVersion(object[] fields, int versionProperty, IVersionType versionType, bool? force,
																	 ISessionImplementor session)
		{
			object initialVersion = fields[versionProperty];
			if (initialVersion == null || !force.HasValue || force.Value)
			{
				fields[versionProperty] = Seed(versionType, session);
				return true;
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("using initial version: " + initialVersion);
				}
				return false;
			}
		}

		/// <summary>
		/// Set the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="version">The value the version should be set to in the <c>fields</c> parameter.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for persisting the values of the <c>fields</c> parameter.</param>
		public static void SetVersion(object[] fields, object version, IEntityPersister persister)
		{
			if (!persister.IsVersioned)
				return;

			fields[persister.VersionProperty] = version;
		}

		/// <summary>
		/// Get the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="persister">The <see cref="IEntityPersister"/> that is responsible for persisting the values of the <c>fields</c> parameter.</param>
		/// <returns>
		/// The value of the version contained in the <c>fields</c> parameter or null if the
		/// Entity is not versioned.
		/// </returns>
		public static object GetVersion(object[] fields, IEntityPersister persister)
		{
			return persister.IsVersioned ? fields[persister.VersionProperty] : null;
		}

		/// <summary> Do we need to increment the version number, given the dirty properties? </summary>
		/// <param name="dirtyProperties">The array of property indexes which were deemed dirty </param>
		/// <param name="hasDirtyCollections">Were any collections found to be dirty (structurally changed) </param>
		/// <param name="propertyVersionability">An array indicating versionability of each property. </param>
		/// <returns> True if a version increment is required; false otherwise. </returns>
		public static bool IsVersionIncrementRequired(int[] dirtyProperties, bool hasDirtyCollections, bool[] propertyVersionability)
		{
			if (hasDirtyCollections)
				return true;

			for (int i = 0; i < dirtyProperties.Length; i++)
			{
				if (propertyVersionability[dirtyProperties[i]])
					return true;
			}
			return false;
		}
	}
}