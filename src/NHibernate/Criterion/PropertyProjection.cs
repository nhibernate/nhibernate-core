using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	using System.Collections.Generic;

	/// <summary>
	/// A property value, or grouped property value
	/// </summary>
	[Serializable]
	public class PropertyProjection : SimpleProjection
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

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {criteriaQuery.GetType(criteria, propertyName)};
		}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return new SqlString(new object[]
			                     	{
			                     		criteriaQuery.GetColumn(criteria, propertyName),
			                     		" as y",
			                     		loc.ToString(),
			                     		"_"
			                     	});
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			if (!grouped)
			{
				return base.ToGroupSqlString(criteria, criteriaQuery, enabledFilters);
			}
			else
			{
				return new SqlString(criteriaQuery.GetColumn(criteria, propertyName));
			}
		}
	}
}