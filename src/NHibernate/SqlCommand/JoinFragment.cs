using System;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand
{
	/// <summary></summary>
	public enum JoinType
	{
		/// <summary></summary>
		None = -666,
		/// <summary></summary>
		InnerJoin = 0,
		/// <summary></summary>
		FullJoin = -1,
		/// <summary></summary>
		LeftOuterJoin = 1,
		/// <summary></summary>
		RightOuterJoin = 2
	}

	/// <summary>
	/// Represents a SQL <c>JOIN</c>
	/// </summary>
	public abstract class JoinFragment
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		/// <param name="joinType"></param>
		public abstract void AddJoin( string tableName, string alias, string[ ] fkColumns, string[ ] pkColumns, JoinType joinType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="alias"></param>
		public abstract void AddCrossJoin( string tableName, string alias );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragment"></param>
		/// <param name="whereFragment"></param>
		public abstract void AddJoins( SqlString fromFragment, SqlString whereFragment );
		
		/// <summary></summary>
		public abstract SqlString ToFromFragmentString { get; }
		
		/// <summary></summary>
		public abstract SqlString ToWhereFragmentString { get; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columns"></param>
		/// <param name="condition"></param>
		public abstract void AddCondition( string alias, string[ ] columns, string condition );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columns"></param>
		/// <param name="condition"></param>
		/// <param name="conditionType"></param>
		/// <param name="factory"></param>
		public abstract void AddCondition( string alias, string[ ] columns, string condition, IType conditionType, ISessionFactoryImplementor factory );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="fkColumns"></param>
		/// <param name="pkColumns"></param>
		public abstract void AddCondition( string alias, string[ ] fkColumns, string[ ] pkColumns );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public abstract void AddCondition( string condition );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public abstract void AddCondition( SqlString condition );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragmentString"></param>
		public abstract void AddFromFragmentString( SqlString fromFragmentString );

		/// <summary></summary>
		public abstract JoinFragment Copy();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ojf"></param>
		public virtual void AddFragment( JoinFragment ojf )
		{
			AddJoins( ojf.ToFromFragmentString, ojf.ToWhereFragmentString );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromFragment"></param>
		/// <param name="whereFragment"></param>
		[Obsolete( "should use SqlString instead" )]
		public virtual void AddJoins( string fromFragment, string whereFragment )
		{
			this.AddJoins( new SqlString( fromFragment ), new SqlString( whereFragment ) );
		}

	}
}