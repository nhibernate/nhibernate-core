using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3772 {
	public static class Utils {
		public static void AddRange<T>(ICollection<T> collection, IEnumerable<T> items) {
			foreach (var item in items) {
				collection.Add(item);
			}
		}
	}
}