namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using Type;
	using Util;

	[Serializable]
	public class ConditionalProjection : SimpleProjection
	{
		private readonly ICriterion criterion;
		private readonly IProjection whenTrue;
		private readonly IProjection whenFalse;

		public ConditionalProjection(ICriterion criterion, IProjection whenTrue, IProjection whenFalse)
		{
			this.whenTrue = whenTrue;
			this.whenFalse = whenFalse;
			this.criterion = criterion;
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlString condition = criterion.ToSqlString(criteria, criteriaQuery, enabledFilters);
			SqlString ifTrue = whenTrue.ToSqlString(criteria, position + GetHashCode() + 1, criteriaQuery, enabledFilters);
			ifTrue = StringHelper.RemoveAsAliasesFromSql(ifTrue);
			SqlString ifFalse = whenFalse.ToSqlString(criteria, position + GetHashCode() + 2, criteriaQuery, enabledFilters);
			ifFalse = StringHelper.RemoveAsAliasesFromSql(ifFalse);
			return new SqlStringBuilder()
				.Add("(")
				.Add("case when ")
				.Add(condition)
				.Add(" then ")
				.Add(ifTrue)
				.Add(" else ")
				.Add(ifFalse)
				.Add(" end")
				.Add(")")
				.Add(" as ")
				.Add(GetColumnAliases(position)[0])
				.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType[] trueTypes = whenTrue.GetTypes(criteria, criteriaQuery);
			IType[] falseTypes = whenFalse.GetTypes(criteria, criteriaQuery);

			bool areEqual = trueTypes.Length == falseTypes.Length;
			if (trueTypes.Length == falseTypes.Length)
			{
				for (int i = 0; i < trueTypes.Length; i++)
				{
					if(trueTypes[i] != falseTypes[i])
					{
						areEqual = false;
						break;
					}
				}
			}
			if(areEqual == false)
			{
				string msg = "Both true and false projections must return the same types."+ Environment.NewLine +
				             "But True projection returns: ["+StringHelper.Join(", ", trueTypes) +"] "+ Environment.NewLine+
				             "And False projection returns: ["+StringHelper.Join(", ", falseTypes)+ "]";

				throw new HibernateException(msg);
			}

			return trueTypes;
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> tv = new List<TypedValue>();
			tv.AddRange(criterion.GetTypedValues(criteria, criteriaQuery));
			tv.AddRange(whenTrue.GetTypedValues(criteria, criteriaQuery));
			tv.AddRange(whenFalse.GetTypedValues(criteria, criteriaQuery));
			return tv.ToArray();
		}
	}
}