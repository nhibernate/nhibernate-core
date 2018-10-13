using System.Collections.Generic;

namespace NHibernate.Test.PropertyRef {
	public class ManyA {
		public ManyA () {
			ManyBs = new List<ManyB>();
		}
		
		public long Id { get; set; }
		public long Number { get; set; }
		public string Value { get; set; }
		public IList<ManyB> ManyBs { get; set; }
	}
}
