namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using SqlTypes;
	using Type;
	using Util;

	/// <summary>
	/// Casting a value from one type to another, at the database
	/// level
	/// </summary>
	[Serializable]
	public class CastProjection : SimpleProjection 
	{
		private readonly IType type;
		private readonly IProjection projection;

		public CastProjection(IType type, IProjection projection)
		{
			this.type = type;
			this.projection = projection;
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			SqlType[] sqlTypeCodes = type.SqlTypes(factory);
			if (sqlTypeCodes.Length != 1)
			{
				throw new QueryException("invalid Hibernate type for CastProjection");
			}
			string sqlType = factory.Dialect.GetCastTypeName(sqlTypeCodes[0]);
			int loc = position*GetHashCode();
			SqlString val = projection.ToSqlString(criteria, loc, criteriaQuery,enabledFilters);
			val = SqlStringHelper.RemoveAsAliasesFromSql(val);

			return new SqlString("cast( ", val, " as ", sqlType, ") as ", GetColumnAliases(position, criteria, criteriaQuery)[0]);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[]{ type };
		}

		public override NHibernate.Engine.TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypedValues(criteria, criteriaQuery);
		}

		public override bool IsGrouped
		{
			get
			{
				return projection.IsGrouped;
			}
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return projection.ToGroupSqlString(criteria, criteriaQuery, enabledFilters);
		}
	}
}
