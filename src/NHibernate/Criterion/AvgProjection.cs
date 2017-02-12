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
		public AvgProjection(string propertyName) : base("avg", propertyName) {}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			SqlType[] sqlTypeCodes = NHibernateUtil.Double.SqlTypes(factory);
			string sqlType = factory.Dialect.GetCastTypeName(sqlTypeCodes[0]);

			var sql = new SqlStringBuilder().Add(aggregate).Add("(");
			sql.Add("cast(");
			if (projection != null)
			{
				sql.Add(SqlStringHelper.RemoveAsAliasesFromSql(projection.ToSqlString(criteria, loc, criteriaQuery, enabledFilters)));
			}
			else
			{
				sql.Add(criteriaQuery.GetColumn(criteria, propertyName));
			}
			sql.Add(" as ").Add(sqlType).Add(")");
			sql.Add(") as ").Add(GetColumnAliases(loc, criteria, criteriaQuery)[0]);
			return sql.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Double};
		}
	}
}
