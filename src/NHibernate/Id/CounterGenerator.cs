using System;
using NHibernate.Engine;

namespace NHibernate.Id {
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns a <c>long</c> constructed from the system
	/// time and a counter value. Not safe for use in a clustser!
	/// </summary>
	public class CounterGenerator : IIdentifierGenerator {
		
		private static short counter = 0;

		protected short Count {
			get {
				lock(typeof(CounterGenerator)) {
					if (counter<0) counter=0;
					return counter++;
				}
			}
		}

		public object Generate(ISessionImplementor cache, object obj) {
			return ( DateTime.Now.Ticks << 16 ) + Count;
		}

	}
}
