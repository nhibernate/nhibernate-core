using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary></summary>
	public enum JoinType
	{
		None = -666,
		InnerJoin = 0,
		FullJoin = 4,
		LeftOuterJoin = 1,
		RightOuterJoin = 2
	}

	/// <summary>
	/// Represents a SQL <c>JOIN</c>
	/// </summary>
	public abstract class JoinFragment
	{
		private bool hasFilterCondition = false;

		public abstract void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType);

		public abstract void AddJoin(string tableName, string alias, string[] fkColumns, string[] pkColumns, JoinType joinType,
		                             string on);

		public abstract void AddCrossJoin(string tableName, string alias);

		public abstract void AddJoins(SqlString fromFragment, SqlString whereFragment);

		public abstract SqlString ToFromFragmentString { get; }

		public abstract SqlString ToWhereFragmentString { get; }

		public abstract bool AddCondition(string condition);

		public abstract bool AddCondition(SqlString condition);

		public abstract void AddFromFragmentString(SqlString fromFragmentString);

		public virtual void AddFragment(JoinFragment ojf)
		{
			AddJoins(ojf.ToFromFragmentString, ojf.ToWhereFragmentString);
		}

		protected bool AddCondition(SqlStringBuilder buffer, string on)
		{
			if (StringHelper.IsNotEmpty(on))
			{
				if (!on.StartsWith(" and"))
				{
					buffer.Add(" and ");
				}
				buffer.Add(on);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected bool AddCondition(SqlStringBuilder buffer, SqlString on)
		{
			if (StringHelper.IsNotEmpty(on))
			{
				if (!on.StartsWithCaseInsensitive(" and"))
				{
					buffer.Add(" and ");
				}
				buffer.Add(on);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasFilterCondition
		{
			get { return hasFilterCondition; }
			set { hasFilterCondition = value; }
		}
	}
}