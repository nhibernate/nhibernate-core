namespace NHibernate.Expressions
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using Type;

	/// <summary>
	/// This is useful if we want to send a value
	/// to the database
	/// </summary>
	[Serializable]
	public class ConstantProjection : SimpleProjection
	{
		private readonly object value;

		public ConstantProjection(object value)
		{
			this.value = value;
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return new SqlStringBuilder()
				.AddParameter()
				.Add(" as ")
				.Add(GetColumnAliases(position)[0])
				.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.GuessType(value)};
		}

		public override NHibernate.Engine.TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[]
				{
					new TypedValue(NHibernateUtil.GuessType(value), value),
				};
		}
	}
}