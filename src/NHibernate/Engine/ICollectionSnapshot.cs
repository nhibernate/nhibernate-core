using System;

namespace NHibernate.Engine {
	
	public interface ICollectionSnapshot {
		object Key { get; }
		string Role { get; }
		object Snapshot { get; }
		bool Dirty { get; }
		void SetDirty();
		bool IsInitialized{get;}
	}
}
