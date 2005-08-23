using System;

using log4net;
using NHibernate.Persister;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// Utility methods for managing versions and timestamps
	/// </summary>
	public class Versioning
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( Versioning ) );


		/// <summary>
		/// Increment the given version number
		/// </summary>
		/// <param name="version">The value of the current version.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <returns>Returns the next value for the version.</returns>
		public static object Increment( object version, IVersionType versionType )
		{
			object next = versionType.Next( version );
			if( log.IsDebugEnabled )
			{
				log.Debug( "Incrementing: " + version + " to " + next );
			}
			return next;
		}

		/// <summary>
		/// Create an initial version number
		/// </summary>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <returns>A seed value to initialize the versioned property with.</returns>
		public static object Seed( IVersionType versionType )
		{
			object seed = versionType.Seed;
			if( log.IsDebugEnabled )
			{
				log.Debug( "Seeding: " + seed );
			}
			return seed;
		}

		private static bool IsNegativeNumber( object obj )
		{
			if( !( obj is IConvertible ) )
			{
				return false;
			}

			try
			{
				return Convert.ToInt64( obj ) < 0L;
			}
			catch( InvalidCastException )
			{
				return false;
			}
		}

		/// <summary>
		/// Seed the given instance state snapshot with an initial version number
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="versionProperty">The index of the version property in the <c>fields</c> parameter.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <returns><c>true</c> if the version property needs to be seeded with an initial value.</returns>
		public static bool SeedVersion( object[ ] fields, int versionProperty, IVersionType versionType, bool force )
		{
			object initialVersion = fields[ versionProperty ];
			if( initialVersion == null || force )
			{
				fields[ versionProperty ] = Seed( versionType );
				return true;
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "using initial version: " + initialVersion );
				}
				return false;
			}
		}

		/// <summary>
		/// Gets the value of the version.
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="versionProperty">The index of the version property in the <c>fields</c> parameter.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		/// <returns>The value of the version.</returns>
		private static object GetVersion( object[ ] fields, int versionProperty, IVersionType versionType )
		{
			return fields[ versionProperty ];
		}

		/// <summary>
		/// Sets the value of the version.
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="version">The value the version should be set to in the <c>fields</c> parameter.</param>
		/// <param name="versionProperty">The index of the version property in the <c>fields</c> parameter.</param>
		/// <param name="versionType">The <see cref="IVersionType"/> of the versioned property.</param>
		private static void SetVersion( object[ ] fields, object version, int versionProperty, IVersionType versionType )
		{
			fields[ versionProperty ] = version;
		}

		/// <summary>
		/// Set the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="version">The value the version should be set to in the <c>fields</c> parameter.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for persisting the values of the <c>fields</c> parameter.</param>
		public static void SetVersion( object[ ] fields, object version, IClassPersister persister )
		{
			SetVersion( fields, version, persister.VersionProperty, persister.VersionType );
		}

		/// <summary>
		/// Get the version number of the given instance state snapshot
		/// </summary>
		/// <param name="fields">An array of objects that contains a snapshot of a persistent object.</param>
		/// <param name="persister">The <see cref="IClassPersister"/> that is responsible for persisting the values of the <c>fields</c> parameter.</param>
		/// <returns>
		/// The value of the version contained in the <c>fields</c> parameter or null if the
		/// Entity is not versioned.
		/// </returns>
		public static object GetVersion( object[ ] fields, IClassPersister persister )
		{
			return persister.IsVersioned ? GetVersion( fields, persister.VersionProperty, persister.VersionType ) : null;
		}


	}
}