using System;

namespace NHibernate.Sql 
{
	public enum JoinType 
	{
		None = -666,
		InnerJoin = 0,
		FullJoin = -1,
		LeftOuterJoin = 1,
		RightOuterJoin = 2
	}
	
	/// <summary>
	/// Represents a SQL <c>JOIN</c>
	/// </summary>
	public abstract class JoinFragment 
	{
		
		public abstract void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType);
		public abstract void AddCrossJoin(string tableName, string alias);
		public abstract void AddJoins(string fromFragment, string whereFragment);
		public abstract string ToFromFragmentString { get; }
		public abstract string ToWhereFragmentString { get; }
		public abstract void AddCondition(string alias, string[] columns, string condition);
		public abstract void AddCondition(string alias, string[] fkColumns, string[] pkColumns);
		public abstract void AddCondition(string condition);

		public abstract JoinFragment Copy();

		public void AddFragment(JoinFragment ojf) 
		{
			AddJoins( ojf.ToFromFragmentString, ojf.ToWhereFragmentString );
		}

	}
}
