using System;

namespace NHibernate.Sql {
	
	public abstract class OuterJoinFragment {
		
		public abstract void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, bool innerJoin);
		public abstract void AddJoins(string fromFragment, string whereFragment);
		public abstract string ToFromFragmentString();
		public abstract string ToWhereFragmentString();

	}
}
