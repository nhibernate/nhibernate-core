using System;
using System.Data;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Ps {

	/// <summary>
	/// A cache for <c>PreparedStatement</c>s that is reasonably efficient for small
	/// maximum cache sizes
	/// </summary>
	public class PreparedStatementCache {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PreparedStatementCache));

		private int maxSize;
		private ArrayList open = new ArrayList();
		
		class Entry {
			public string sql;
			public IDbConnection conn;

			public Entry(string sql, IDbConnection conn) {
				this.sql = sql; this.conn = conn;
			}
			public override bool Equals(object other) {
				Entry oe = (Entry) other;
				return oe.conn==conn && oe.sql.Equals(sql);
			}
			public override int GetHashCode() {
				return sql.GetHashCode();
			}
		}
		private IDictionary cache = new Hashtable();
		private IDictionary entryMap = IdentityMap.Instantiate();
		private IList entries = new ArrayList();
		private short reapCounter = 0;

		public PreparedStatementCache(int maxSize) {
			this.maxSize = maxSize;
			log.Info("prepared statement cache size: " + maxSize);
		}

		public IDbCommand GetPreparedStatement(string sql, IDbConnection conn) {
			Entry e = new Entry(sql, conn);

			IDbCommand ps;
			lock(this) {
				if ( log.IsDebugEnabled ) open.Add(sql);

				ps = (IDbCommand) cache[e];
				cache.Remove(e);

				if (ps != null) entries.Remove(entryMap[ps]);
			}

			if (ps == null) {
				if (log.IsDebugEnabled) log.Debug("preparing statement: " + sql);
				try {
					ps = conn.CreateCommand();
					ps.CommandText = sql;
					ps.CommandType = CommandType.Text;
				} catch(Exception exp) {
					throw new ADOException("could not create command", exp);
				}

				lock (this) {
					entryMap[ps] = e;
				}
			} else {
				if ( log.IsDebugEnabled ) log.Debug("returning cached statement: " + sql);
			}

			return ps;
		}

		public void ClosePreparedStatement(IDbCommand ps) {

			log.Debug("recaching");

			IDbCommand old = null;
			lock(this) {
				Entry e = (Entry) entryMap[ps];

				if ( log.IsDebugEnabled ) open.Remove(e.sql);

				cache[e] = ps;
				old = ps;

				if (old == null) {
					reapCounter++;
				} else {
					entries.Remove( entryMap[old] );
					entryMap.Remove(old);
				}

				entries.Add(e);
			}

			if (old != null) {
				try {
					old.Dispose();
				} catch(Exception e) {
					log.Warn("could not dispose statemnt", e);
				}
			}

			Reap();

			if ( log.IsDebugEnabled ) {
				lock(this) {
					log.Debug( "total checked-out statements: " + ( entryMap.Count - entries.Count ) );
					log.Debug( "checked out: " + open );
				}
			}
		}

		public void CloseAll(IDbConnection conn) {

			log.Debug("closing all statements for connection");

			ArrayList templist = new ArrayList(25);

			lock(this) {
				foreach(Entry e in entries) {
					if (e.conn == conn) {
						templist.Add( cache[e] );
						cache.Remove(e);
					}
				}
			}

			Close(templist);
		}

		private void Reap() {
			ArrayList templist = new ArrayList(25);

			lock(this) {
				if (reapCounter<20) return;
				int size = entries.Count;
				while ( maxSize < size-- ) {
					templist.Add( entries[0] );
					cache.Remove( entries[0] );
					entries.RemoveAt(0);
				}
				reapCounter = 0;
			}

			if ( log.IsDebugEnabled ) log.Debug( "reaping: " + templist.Count + " statements" );

			Close(templist);
		}

		private void Close(IList templist) {
			foreach( IDbCommand ps in templist ) {
				try {
					entryMap.Remove(ps);
					ps.Dispose();
				} catch (Exception e) {
					log.Warn("could not close statement", e);
				}
			}
		}

		protected void Finalize() { //TODO: this needs to be a .net finalize
			log.Info("Finalizing dereferenced prepared statement cache");

			foreach( IDbCommand ps in cache.Values ) {
				try {
					ps.Dispose();
				} catch(Exception e) {
					log.Debug("could not close statemtn", e);
				}
			}
		}
	}
}
