using System;

namespace NHibernate.SqlCommand
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
		public abstract void AddJoins(SqlString fromFragment, SqlString whereFragment);
		public abstract SqlString ToFromFragmentString { get; }
		public abstract SqlString ToWhereFragmentString { get; }
		public abstract void AddCondition(string alias, string[] columns, string condition);
		public abstract void AddCondition(string alias, string[] fkColumns, string[] pkColumns);
		public abstract void AddCondition(string condition);
		
		public virtual void AddCondition(SqlString condition)
		{
			//TODO: hack with ToString()
			AddCondition( condition.ToString() );
		}

		public abstract JoinFragment Copy();

		public virtual void AddFragment(JoinFragment ojf) 
		{
			AddJoins( ojf.ToFromFragmentString, ojf.ToWhereFragmentString );
		}

		[Obsolete("should use SqlString instead")]
		public virtual void AddJoins(string fromFragment, string whereFragment) 
		{
			this.AddJoins( new SqlString(fromFragment), new SqlString(whereFragment) );
		}

	}
}
