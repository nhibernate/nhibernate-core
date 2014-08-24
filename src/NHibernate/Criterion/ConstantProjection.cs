using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// This is useful if we want to send a value to the database
	/// </summary>
	[Serializable]
	public class ConstantProjection : SimpleProjection
	{
		private readonly object value;
		private readonly TypedValue typedValue;

		public ConstantProjection(object value) : this(value, NHibernateUtil.GuessType(value.GetType()))
		{
		}

		public ConstantProjection(object value, IType type)
		{
			this.value = value;
			typedValue = new TypedValue(type, this.value, EntityMode.Poco);
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			throw new InvalidOperationException("not a grouping projection");
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return new SqlString(
				criteriaQuery.NewQueryParameter(typedValue).Single(),
				" as ",
				GetColumnAliases(position, criteria, criteriaQuery)[0]);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { typedValue.Type };
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] { typedValue };
		}
	}
}
