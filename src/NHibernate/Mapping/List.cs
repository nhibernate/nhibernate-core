using System;
using NHibernate.Type;

namespace NHibernate.Mapping {
	
	/// <summary>
	/// A list has a primary key consisting of the key columns + index column
	/// </summary>
	public abstract class List : IndexedCollection {
		
		public List(PersistentClass owner) : base(owner) { }

		
	}
}
