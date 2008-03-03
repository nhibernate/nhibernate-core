namespace NHibernate.Test.Criteria
{
	using System.Collections.Generic;
	using Engine;
	using Expressions;
	using SqlCommand;
	using Type;

	public class AddNumberProjection : SimpleProjection
	{
		private readonly string propertyName;
		private readonly int numberToAdd;

		public AddNumberProjection(string propertyName, int numberToAdd)
		{
			this.propertyName = propertyName;
			this.numberToAdd = numberToAdd;
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			string[] projection = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
			return new SqlStringBuilder()
				.Add("(")
				.Add(projection[0])
				.Add(" + ")
				.AddParameter()
				.Add(") as ")
				.Add(GetColumnAliases(0)[0])
				.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType projection = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);
			return new IType[] {projection};
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {new TypedValue(NHibernateUtil.Int32, numberToAdd, EntityMode.Poco)};
		}
	}
}