using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	[Serializable]
	public class AvgProjection : AggregateProjection
	{
		public AvgProjection(IProjection projection) : base("avg", projection) {}
		public AvgProjection(String propertyName) : base("avg", propertyName) {}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery,
		                                      IDictionary<string, IFilter> enabledFilters)
		{
			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			SqlType[] sqlTypeCodes = NHibernateUtil.Double.SqlTypes(factory);
			string sqlType = factory.Dialect.GetCastTypeName(sqlTypeCodes[0]);
			string parameter;
			if (projection != null)
			{
				parameter =
					SqlStringHelper.RemoveAsAliasesFromSql(projection.ToSqlString(criteria, loc, criteriaQuery, enabledFilters)).ToString();
			}
			else
			{
				parameter = criteriaQuery.GetColumn(criteria, propertyName);
			}
			string expression = string.Format("{0}(cast({1} as {2})) as {3}", aggregate, parameter, sqlType,
			                                  GetColumnAliases(loc, criteria, criteriaQuery)[0]);
			return new SqlString(expression);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Double};
		}
	}
}