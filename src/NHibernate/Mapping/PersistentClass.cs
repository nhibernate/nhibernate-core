using System;
using System.Collections;

namespace NHibernate.Mapping {
	
	public abstract class PersistentClass {
		private System.Type persistentClass;
		private string discriminatorValue;
		private ArrayList properties = new ArrayList();
		private Table table;
	}
}
