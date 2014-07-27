using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Test.Criteria
{
	public class AddNumberProjection : SimpleProjection
	{
		private readonly string propertyName;
		private readonly int numberToAdd;
		private readonly TypedValue typedValue;

		public AddNumberProjection(string propertyName, int numberToAdd)
		{
			this.propertyName = propertyName;
			this.numberToAdd = numberToAdd;
			typedValue = new TypedValue(NHibernateUtil.Int32, this.numberToAdd, EntityMode.Poco);
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			string[] projection = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);

			return new SqlStringBuilder()
				.Add("(")
				.Add(projection[0])
				.Add(" + ")
				.Add(criteriaQuery.NewQueryParameter(typedValue).Single())
				.Add(") as ")
				.Add(GetColumnAliases(0, criteria, criteriaQuery)[0])
				.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType projection = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);
			return new IType[] {projection};
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {typedValue};
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			throw new InvalidOperationException("not a grouping projection");
		}
	}
}
