using System;
using System.Data;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Persister;

namespace NHibernate.Loader {
	/// <summary>
	/// Abstract superclass of object loading (and querying) strategies.
	/// </summary>
	/// <remarks>
	/// This class implements useful common funtionality that concrete loaders would delegate to.
	/// It is not intended that this functionality would be directly accessed by client code (Hence,
	/// all methods of this class will eventually be declared <c>Protected</c>
	/// </remarks>
	public abstract class Loader {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Loader));

		/// <summary>
		/// The SQL query string to be called; implemented by all subclasses
		/// </summary>
		protected abstract string SQLString { get; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		protected abstract ILoadable[] Persisters { get; }

		/// <summary>
		/// The suffix identifies a particular column of results in the SQL <c>IDataReader</c>;
		/// implemented by all subclasses
		/// </summary>
		protected abstract string[] Suffixes { get; }

		/// <summary>
		/// An (optional) persister for a collection to be initialized; only collection loaders
		/// return a non-null value
		/// </summary>
		protected abstract CollectionPersister CollectionPersister { get; }

		/// <summary>
		/// What lock mode does this load entities with?
		/// </summary>
		protected virtual LockMode LockMode {
			get { return LockMode.Read; }
		}

		/// <summary>
		/// Are we allowed to do two-phase loading? (we aren't for some special cases like sets
		/// of entities ... actually onlt that one special case)
		/// </summary>
		protected virtual bool AllowTwoPhaseLoad {
			get { return true; }
		}




		}
}
