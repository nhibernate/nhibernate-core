using System;
using System.Collections;

namespace NHibernate.Util {

	
	public class JoinedEnumerable : IEnumerable, IEnumerator {
		private IEnumerator[] enumerators;
		private int current;

		public JoinedEnumerable(IEnumerable[] enumerables) {
			enumerators = new IEnumerator[enumerables.Length];
			for (int i=0; i<enumerables.Length; i++) {
				enumerators[i] = enumerables[i].GetEnumerator();
			}
			this.current = 0;
		}

		public bool MoveNext() {
			for ( ; current<enumerators.Length; current++ ) {
				if ( enumerators[current].MoveNext() ) return true;
			}
			return false;
		}
		public void Reset() {
			for (int i=0; i<enumerators.Length; i++) {
				enumerators[i].Reset();
			}
		}
		public object Current {
			get {
				return enumerators[current].Current;
			}
		}
		public IEnumerator GetEnumerator() {
			this.Reset();
			return this;
		}


	}
}
