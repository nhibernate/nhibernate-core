using System;

namespace NHibernate.Engine {
	
	public interface ICollectionSnapshot {
		object Key { get; }
		string Role { get; }
		object GetSnapshot();
		bool Dirty { get; }
		void SetDirty();
	}
}
