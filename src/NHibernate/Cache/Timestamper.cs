using System;

namespace NHibernate.Cache {
	/// <summary>
	/// Geterates increasing identifiers. 
	/// </summary>
	/// <remarks>
	/// Not valid across multiple application domains. Identifiers are not necessarily
	/// strickly increasing, but usually are.
	///	</remarks>
	public class Timestamper {
		private static short counter = 0;
		private static long time;

		public static long Next() {
			lock(typeof(Timestamper)) {
				long newTime = System.DateTime.Now.Ticks << 16; //is this right?
				if (time < newTime) {
					time = newTime;
					counter = 0;
				} else if (counter < short.MaxValue) {
					counter++;
				}
				return time + counter;
			}
		}
	}
}
