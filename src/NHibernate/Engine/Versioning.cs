using System;
using NHibernate.Persister;
using NHibernate.Type;

namespace NHibernate.Engine {
	/// <summary>
	/// Utility methods for managing versions and timestamps
	/// </summary>
	public class Versioning {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Versioning));
		

		/// <summary>
		/// Increment the given version number
		/// </summary>
		/// <param name="version"></param>
		/// <param name="versionType"></param>
		/// <returns></returns>
		public static object Increment(object version, IVersionType versionType) {
			object next = versionType.Next(version);
			if ( log.IsDebugEnabled ) log.Debug("Incrementing: " + version + " to " + next);
			return next;
		}

		/// <summary>
		/// Create an initial version number
		/// </summary>
		/// <param name="versionType"></param>
		/// <returns></returns>
		public static object Seed(IVersionType versionType) {
			object seed = versionType.Seed;
			if ( log.IsDebugEnabled ) log.Debug("Seeding: " + seed);
			return seed;
		}

		/// <summary>
		/// Seed the given instance state snapshot with an initial version number
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="versionProperty"></param>
		/// <param name="versionType"></param>
		/// <returns></returns>
		public static bool SeedVersion(object[] fields, int versionProperty, IVersionType versionType) {
			if ( fields[versionProperty] == null ) {
				fields[versionProperty] = Seed(versionType);
				return true;
			} else {
				return false;
			}
		}

		private static object GetVersion(object[] fields, int versionProperty, IVersionType versionType) {
			return fields[versionProperty];
		}

		private static void SetVersion(object[] fields, object version, int versionProperty, IVersionType versionType) {
			fields[versionProperty] = version;
		}

		/// <summary>
		/// Set the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="version"></param>
		/// <param name="persister"></param>
		public static void SetVersion(object[] fields, object version, IClassPersister persister) {
			SetVersion( fields, version, persister.VersionProperty, persister.VersionType );
		}

		/// <summary>
		/// Get the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields"></param>
		/// <param name="persister"></param>
		/// <returns></returns>
		public static object GetVersion(object[] fields, IClassPersister persister) {
			return persister.IsVersioned ? GetVersion( fields, persister.VersionProperty, persister.VersionType) : null;
		}




	}
}
