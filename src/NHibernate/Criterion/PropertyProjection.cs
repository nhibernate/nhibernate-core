using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A property value, or grouped property value
	/// </summary>
	[Serializable]
	public class PropertyProjection : SimpleProjection, IPropertyProjection
	{
		private string propertyName;
		private bool grouped;

		protected internal PropertyProjection(string propertyName, bool grouped)
		{
			this.propertyName = propertyName;
			this.grouped = grouped;
		}

		protected internal PropertyProjection(string propertyName)
			: this(propertyName, false)
		{
		}

		public string PropertyName
		{
			get { return propertyName; }
		}

		public override string ToString()
		{
			return propertyName;
		}

		public override bool IsGrouped
		{
			get { return grouped; }
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {criteriaQuery.GetType(criteria, propertyName)};
		}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder s = new SqlStringBuilder();
			string[] cols = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
			for (int i = 0; i < cols.Length; i++)
			{
				s.Add(cols[i]);
				s.Add(" as y");
				s.Add((loc + i).ToString());
				s.Add("_");
				if (i < cols.Length - 1)
					s.Add(", ");
			}
			return s.ToSqlString();
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			if (!grouped)
			{
				throw new InvalidOperationException("not a grouping projection");
			}
			return new SqlString(StringHelper.Join(",", criteriaQuery.GetColumns(criteria, propertyName)));
		}
	}
}