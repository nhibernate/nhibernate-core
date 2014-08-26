using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class FilterMapper : IFilterMapper
	{
		private readonly HbmFilter filter;

		public FilterMapper(string filterName, HbmFilter filter)
		{
			if (filterName == null)
			{
				throw new ArgumentNullException("filterName");
			}
			if (string.Empty.Equals(filterName.Trim()))
			{
				throw new ArgumentOutOfRangeException("filterName", "Invalid filter-name: the name should contain no blank characters.");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.filter = filter;
			this.filter.name = filterName;
		}

		#region Implementation of IFilterMapper

		public void Condition(string sqlCondition)
		{
			if (sqlCondition == null || string.Empty.Equals(sqlCondition) || string.Empty.Equals(sqlCondition.Trim()))
			{
				filter.condition = null;
				filter.Text = null;
				return;
			}
			string[] conditionLines = sqlCondition.Split(StringHelper.LineSeparators, StringSplitOptions.None);
			if (conditionLines.Length > 1)
			{
				filter.Text = conditionLines;
				filter.condition = null;
			}
			else
			{
				filter.condition = sqlCondition;
				filter.Text = null;
			}
		}

		#endregion
	}
}