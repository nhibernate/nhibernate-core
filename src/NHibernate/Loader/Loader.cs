using System;
using System.Data;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Persister;
using NHibernate.Type;


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
		public abstract string SQLString { get; }

		/// <summary>
		/// An array of persisters of entity classes contained in each row of results;
		/// implemented by all subclasses
		/// </summary>
		public abstract ILoadable[] Persisters { get; }

		/// <summary>
		/// The suffix identifies a particular column of results in the SQL <c>IDataReader</c>;
		/// implemented by all subclasses
		/// </summary>
		public abstract string[] Suffixes { get; set; }

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

		/// <summary>
		/// Execute an SQL query and attempt to instantiate instances of the class mapped by the given
		/// persister from each row of the <c>IDataReader</c>.
		/// </summary>
		/// <remarks>
		/// If an object is supplied, will attempt to initialize that object. if a collection is supplied,
		/// attemp to initialize that collection
		/// </remarks>
		/// <param name="session"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalID"></param>
		/// <param name="optionalCollection"></param>
		/// <param name="optionalCollectionOwner"></param>
		/// <param name="returnProxies"></param>
		/// <param name="selection"></param>
		/// <param name="namedParms"></param>
		/// <returns></returns>
		private IList DoFind(
			ISessionImplementor session,
			object[] values,
			IType[] types,
			object optionalObject,
			object optionalID,
			PersistentCollection optionalCollection,
			object optionalCollectionOwner,
			bool returnProxies,
			RowSelection selection,
			IDictionary namedParms) {

			int maxRows = (selection==null || selection.MaxRows==0) ?
				int.MaxValue : selection.MaxRows;

			ILoadable[] persisters = Persisters;
			int cols = persisters.Length;
			bool collection = CollectionPersister!=null;
			bool twoPhaseLoad = AllowTwoPhaseLoad && cols > 0;
			string[] suffixes = Suffixes;

			ArrayList hydratedObjects = twoPhaseLoad ? new ArrayList() : null;

			Key optionalObjectKey;
			if (optionalObject!=null) {
				optionalObjectKey = new Key(optionalID, session.GetPersister(optionalObject) );
			} else {
				optionalObjectKey = null;
			}

			IList results = new ArrayList();

			IDbCommand st = PrepareQueryStatement( SQLString, values, types, selection, false, session );
			IDataReader rs = GetResultSet(st, namedParms, selection, session);

			try {

				Key[] keys = new Key[cols];
				bool[] hydrate = new bool[cols];

				for (int count=0; count<maxRows && rs.Read(); count++) {
					for (int i=0; i<cols; i++) {
						keys[i] = GetKeyFromResultSet( persisters[i], suffixes[i], (i==cols-1) ? optionalID : null, rs, session );
					}

					//this call is side-effecty
					object[] row = GetRow(rs, persisters, suffixes, keys, hydrate, optionalObject, optionalObjectKey, session);

					for (int i=0; i<cols; i++) {
						if ( hydrate[i] ) {

							// grab its state from the resultset and keep it in the sesion
							// (but don't yet initialize the object itself)
							LoadFromResultSet( rs, row[i], keys[i].Identifier, suffixes[i], session);

							if (twoPhaseLoad) {
								//materialize associations (and initiailize the object) later
								hydratedObjects.Add( row[i] );
							} else {
								//materialize assocaitions (and initialize the object) now
								session.InitializeEntity( row[i] );
							}
						}

						// now get the proxy, after the call to initializeentity
						if (returnProxies) row[i] = session.ProxyFor( persisters[i], keys[i], row[i] );
					}

					results.Add( GetResultColumnOrRow(row, rs, session) );

					if (collection) optionalCollection.ReadFrom( rs, CollectionPersister, optionalCollectionOwner );
				}
			} catch (Exception e) {
				throw e;
			} finally {
				try {
					rs.Close();
				} finally {
					ClosePreparedStatement(st, selection, session);
				}
			}

			if (twoPhaseLoad) {
				foreach(object obj in hydratedObjects) {
					session.InitializeEntity( obj );
				}
			}

			return results;
		}

		protected object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) {
			return row;
		}

		/// <summary>
		/// Read a row of <c>Key</c>s from the <c>IDataReader</c> into the given array.
		/// </summary>
		/// <remarks>
		/// Warning: this method is side-effecty. If an <c>id</c> is given, don't bother going
		/// to the <c>IDataReader</c>
		/// </remarks>
		/// <param name="persister"></param>
		/// <param name="suffix"></param>
		/// <param name="id"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private Key GetKeyFromResultSet(ILoadable persister, string suffix, object id, IDataReader rs, ISessionImplementor session) {
			if (id==null) {
				//todo: we can cache these on this object, from the construcotr
				string[] keyColNames = StringHelper.Suffix( persister.IdentifierColumnNames, suffix);
				StringHelper.UnQuoteInPlace(keyColNames);

				id = persister.IdentifierType.NullSafeGet(rs, keyColNames, session, null);
			}

			return (id==null) ? null : new Key(id, persister);
		}

		/// <summary>
		/// Resolve any ids for currently loaded objects, duplications within the <c>IDataReader</c>,
		/// etc. Instanciate empty objects to be initialized from the <c>IDataReader</c>. Return an
		/// array of objects (a row of results) and an array of booleans (by side-effect) that determine
		/// wheter the corresponding object should be initialized
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persisters"></param>
		/// <param name="suffixes"></param>
		/// <param name="keys"></param>
		/// <param name="hydrate"></param>
		/// <param name="optionalObject"></param>
		/// <param name="optionalobjectKey"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private object[] GetRow(
			IDataReader rs,
			ILoadable[] persisters,
			string[] suffixes,
			Key[] keys,
			bool[] hydrate, //returns values by side-effect
			object optionalObject,
			Key optionalObjectKey,
			ISessionImplementor session) {

			int cols = persisters.Length;
			object[] rowResults = new object[cols];

			for (int i=0; i<cols; i++) {
				object obj = null;
				Key key = keys[i];

				if (keys[i]==null) {
					hydrate[i] = false;
				} else {

					//If the object is already loaded, return the loaded one
					obj = session.GetEntity(key);
					if (obj!=null) {

						//its already loaded so dont need to hydrate it
						hydrate[i] = false;

						if ( !persisters[i].MappedClass.IsAssignableFrom( obj.GetType() ))
							throw new WrongClassException( "loaded object was of wrong class", key.Identifier, persisters[i].MappedClass );
					} else {

						System.Type instanceClass = GetInstanceClass( rs, persisters[i], suffixes[i], key.Identifier, session);

						if (optionalObjectKey!=null && key.Equals(optionalObjectKey)) {
							//its the given optional object
							obj = optionalObject;
						} else {
							//instantiate a new instance
							obj = session.Instantiate( instanceClass, key.Identifier );
						}

						//need to hydrate it
						hydrate[i] = true;

						// so that code is circular-ref safe:
						session.AddUninitializeEntity(key, obj, LockMode );
					}
				}

				rowResults[i] = obj;
			}
			return rowResults;
		}

		/// <summary>
		/// Hydrate an object from the SQL <c>IDataReader</c>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="obj"></param>
		/// <param name="id"></param>
		/// <param name="suffix"></param>
		/// <param name="session"></param>
		private void LoadFromResultSet(IDataReader rs, object obj, object id, string suffix, ISessionImplementor session) {

			if (log.IsDebugEnabled ) log.Debug("Initializing object from DataReader: " + id);

			// Get the persister for the subclass
			ILoadable persister = (ILoadable) session.GetPersister(obj);

			object[] values = Hydrate(rs, id, obj, persister, session, suffix);
			session.PostHydrate(persister, id, values, obj, LockMode);
		}

		/// <summary>
		/// Determine the concrete class of an instance for the <c>IDataReader</c>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="persister"></param>
		/// <param name="suffix"></param>
		/// <param name="id"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		private System.Type GetInstanceClass(IDataReader rs, ILoadable persister, string suffix, object id, ISessionImplementor session) {

			System.Type topClass = persister.MappedClass;

			if ( persister.HasSubclasses ) {
				string col = StringHelper.UnQuote (
					StringHelper.Suffix ( persister.DiscriminatorColumnName, suffix )
					);

				// code to handle subclasses of topClass
				object discriminatorValue = persister.DiscriminatorType.NullSafeGet(rs, col, session, null);

				System.Type result = persister.GetSubclassForDiscriminatorValue(discriminatorValue);

				if (result==null) throw new WrongClassException("Discriminator: " + discriminatorValue, id, topClass);

				return result;
			} else {
				return topClass;
			}
		}

		/// <summary>
		/// Unmarshall the fields of a persistent instance from a result set
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="id"></param>
		/// <param name="obj"></param>
		/// <param name="persister"></param>
		/// <param name="session"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		private object[] Hydrate(IDataReader rs, object id, object obj, ILoadable persister, ISessionImplementor session, string suffix) {
			if (log.IsDebugEnabled ) log.Debug("Hydrating entity: " + persister.ClassName + '#' + id);

			IType[] types = persister.PropertyTypes;
			object[] values = new object[ types.Length ];

			for (int i=0; i<types.Length; i++) {
				string[] cols = StringHelper.Suffix(persister.GetPropertyColumnNames(i), suffix);
				StringHelper.UnQuote(cols);

				values[i] = types[i].Hydrate( rs, cols, session, obj);
			}
			return values;
		}

		/// <summary>
		/// Advance the cursor to the first required row of the <c>ResultSet</c>
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="selection"></param>
		/// <param name="session"></param>
		protected void Advance(IDataReader rs, RowSelection selection, ISessionImplementor session) {
			if (selection==null) {
				return;
			} else {
				int first = selection.FirstRow;
				if (first!=0) {
					for (int m=0; m<first; m++) rs.Read();
				}
			}
		}

		/// <summary>
		/// Obtain a <c>PreparedStatemnt</c> and bind parameters
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="values"></param>
		/// <param name="types"></param>
		/// <param name="selection"></param>
		/// <param name="scroll"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected IDbCommand PrepareQueryStatement(string sql, object[] values, IType[] types, RowSelection selection, bool scroll, ISessionImplementor session) {

			IDbCommand st = session.Batcher.PrepareQueryStatement(sql);

			try {
				
				if (selection!=null && selection.Timeout!=0) st.CommandTimeout = selection.Timeout;

				int col=1;
				for (int i=0; i<values.Length; i++) {
					types[i].NullSafeSet( st, values[i], col, session);
					col += types[i].GetColumnSpan( session.Factory );
				}
			} catch (Exception e) {
				ClosePreparedStatement(st, selection, session);
				throw e;
			}

			return st;
		}

		protected void SetMaxRows(IDbCommand st, RowSelection selection) {
			//not implemented
		}

		private IDataReader GetResultSet(IDbCommand st, IDictionary namedParams, RowSelection selection, ISessionImplementor session) {
			try {
				BindNamedParameters(st, namedParams, session);
				SetMaxRows(st, selection);
				IDataReader rs = st.ExecuteReader();
				Advance(rs, selection, session);
				return rs;
			} catch (Exception e) {
				ClosePreparedStatement(st, selection, session);
				throw e;
			}
		}

		protected void ClosePreparedStatement(IDbCommand st, RowSelection selection, ISessionImplementor session) {
			try {
				if (selection!=null) {
					st.CommandTimeout = 0;
				}
			} finally {
				session.Batcher.CloseQueryStatement(st);
			}
		}

		/// <summary>
		/// Bind named parameters to the <c>PreparedStatement</c>
		/// </summary>
		/// <param name="st"></param>
		/// <param name="namedParams"></param>
		/// <param name="session"></param>
		protected virtual void BindNamedParameters(IDbCommand st, IDictionary namedParams, ISessionImplementor session) {
		}


		protected IList LoadEntity(
			ISessionImplementor session,
			object[] values,
			IType[] types,
			object optionalObject,
			object optionalID,
			bool returnProxies) {

			return DoFind(session, values, types, optionalObject, optionalID, null, null, returnProxies, null, null);
		}

		protected IList LoadCollection(
			ISessionImplementor session,
			object id,
			IType type,
			object owner,
			PersistentCollection collection) {

			return DoFind(session, new object[] {id}, new IType[] {type}, null, null, collection, owner, true, null, null);
		}

		protected IList Find(
			ISessionImplementor session,
			object[] values,
			IType[] types,
			bool returnProxies,
			RowSelection selection,
			IDictionary namedParams) {

			return DoFind(session, values, types, null, null, null, null, returnProxies, selection, namedParams);
		}




	}
}
