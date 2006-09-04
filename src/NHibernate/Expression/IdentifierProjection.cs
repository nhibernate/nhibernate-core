using System;
using System.Text;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	public class IdentifierProjection : SimpleProjection
	{
		bool grouped;

		protected internal IdentifierProjection(bool grouped)
		{
			this.grouped = grouped;
		}

		protected internal IdentifierProjection() : this(false) { }

		public override string ToString()
		{
			return "id";
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { criteriaQuery.GetIdentifierType(criteria) };
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
            SqlStringBuilder buf = new SqlStringBuilder();
			string[] cols = criteriaQuery.GetIdentifierColumns(criteria);
			for (int i = 0; i < cols.Length; i++)
			{
				buf.Add(cols[i])
					.Add(" as y")
					.Add((position + i).ToString())
					.Add("_");
			}
			return buf.ToSqlString();
		}

		public override bool IsGrouped { get { return grouped; } }

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return (grouped) ? 
                new SqlString(NHibernate.Util.StringHelper.Join(",", criteriaQuery.GetIdentifierColumns(criteria)))
				: base.ToGroupSqlString(criteria, criteriaQuery);
		}

	}
}
using System;
using System.Text;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	public class IdentifierProjection : SimpleProjection
	{
		bool grouped;

		protected internal IdentifierProjection(bool grouped)
		{
			this.grouped = grouped;
		}

		protected internal IdentifierProjection() : this(false) { }

		public override string ToString()
		{
			return "id";
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { criteriaQuery.GetIdentifierType(criteria) };
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
            SqlStringBuilder buf = new SqlStringBuilder();
			string[] cols = criteriaQuery.GetIdentifierColumns(criteria);
			for (int i = 0; i < cols.Length; i++)
			{
				buf.Add(cols[i])
					.Add(" as y")
					.Add((position + i).ToString())
					.Add("_");
			}
			return buf.ToSqlString();
		}

		public override bool IsGrouped { get { return grouped; } }

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return (grouped) ? 
                new SqlString(NHibernate.Util.StringHelper.Join(",", criteriaQuery.GetIdentifierColumns(criteria)))
				: base.ToGroupSqlString(criteria, criteriaQuery);
		}

	}
}
