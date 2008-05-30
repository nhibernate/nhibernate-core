using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	[Serializable]
	public class NaturalIdentifier: ICriterion
	{
		private readonly Junction conjunction = new Conjunction();

		public SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                             IDictionary<string, IFilter> enabledFilters)
		{
			return conjunction.ToSqlString(criteria, criteriaQuery, enabledFilters);
		}

		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return conjunction.GetTypedValues(criteria, criteriaQuery);
		}

		public IProjection[] GetProjections()
		{
			return conjunction.GetProjections();
		}

		public NaturalIdentifier Set(string property, object value)
		{
			conjunction.Add(Restrictions.Eq(property, value));
			return this;
		}
	}
}
