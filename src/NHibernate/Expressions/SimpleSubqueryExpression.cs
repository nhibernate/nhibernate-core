using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expressions
{
	/// <summary>
	/// A comparison between a constant value and the the result of a subquery
	/// </summary>
	[Serializable]
	public class SimpleSubqueryExpression : SubqueryExpression
	{
		private Object value;

		internal SimpleSubqueryExpression(Object value, String op, String quantifier, DetachedCriteria dc)
			: base(op, quantifier, dc)

		{
			this.value = value;
		}


		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			TypedValue[] superTv = base.GetTypedValues(criteria, criteriaQuery);
			TypedValue[] result = new TypedValue[superTv.Length + 1];
			superTv.CopyTo(result, 1);
			result[0] = new TypedValue(GetTypes()[0], value, EntityMode.Poco);
			return result;
		}

		protected override SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return SqlString.Parameter;
		}
	}
}