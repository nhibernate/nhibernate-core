using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl {
	/// <summary>
	/// Implements enumerable
	/// </summary>
	internal class EnumerableImpl : IEnumerable, IEnumerator{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EnumerableImpl));
		
		private IDataReader rs;
		private ISessionImplementor sess;
		private IType[] types;
		private bool single;
		private object[] nextResults;
		private bool hasNext;
		private string[][] names;

		public EnumerableImpl(IDataReader rs, ISessionImplementor sess, IType[] types, string[][] columnNames) {
			this.rs = rs;
			this.sess = sess;
			this.types = types;
			this.names = columnNames;

			single = types.Length==1;

			
		}

		private void PostNext(bool hasNext) {
			this.hasNext = hasNext;
			if (!hasNext) {
				nextResults = null;
				rs.Close();
			} else {
				nextResults = new object[types.Length];
				for (int i=0; i<types.Length; i++) {
					nextResults[i] = types[i].NullSafeGet(rs, names[i], sess, null);
				}
			}
		}

		
		public IEnumerator GetEnumerator() {
			this.Reset();
			return this;
		}

		public object Current {
			get {
				if (single) {
					return nextResults[0];
				} else {
					return nextResults;
				}
			}
		}

		public bool MoveNext() {
			PostNext(rs.Read());

			return hasNext;
		}

		public void Reset() {
			//can't reset the reader...we are SOL
		}


	}
}
