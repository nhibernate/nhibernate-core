namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using Type;

	/// <summary>
	/// This is useful if we want to send a value to the database
	/// </summary>
	[Serializable]
	public class ConstantProjection : SimpleProjection
	{
		private readonly object value;
		private readonly IType type;
		public ConstantProjection(object value):this(value,NHibernateUtil.GuessType(value.GetType()))
		{
			
		}
		public ConstantProjection(object value,IType type)
		{
			this.value = value;
			this.type = type;
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
			criteriaQuery.AddUsedTypedValues(new TypedValue[] { new TypedValue(type, value, EntityMode.Poco) });
			return new SqlStringBuilder()
				.AddParameter()
				.Add(" as ")
				.Add(GetColumnAliases(position)[0])
				.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] { type };
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] { new TypedValue(type, value, EntityMode.Poco) };
		}
	}
}
