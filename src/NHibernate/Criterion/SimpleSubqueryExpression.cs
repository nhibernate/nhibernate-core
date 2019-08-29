using System;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
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
			var typedValues = GetTypedValues(GetTypes()[0], value);
			TypedValue[] result = new TypedValue[superTv.Length + typedValues.Length];
			superTv.CopyTo(result, 0);
			typedValues.CopyTo(result, superTv.Length);

			return result;
		}

		protected override SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var typedValues = GetTypedValues(GetTypes()[0], value);
			return new SqlStringBuilder()
					.AddParameters(typedValues.Select(tv => criteriaQuery.NewQueryParameter(tv)).SelectMany(x => x).ToList())
					.ToSqlString();
		}

		private static TypedValue[] GetTypedValues(IType type, object value)
		{
			if (!type.IsComponentType)
				return new[] {new TypedValue(type, value)};

			IAbstractComponentType actype = (IAbstractComponentType) type;
			IType[] types = actype.Subtypes;
			var list = new TypedValue[types.Length];
			var propertyValues = value == null
				? null
				: actype.GetPropertyValues(value);
			for (int ti = 0; ti < types.Length; ti++)
			{
				list[ti] = new TypedValue(types[ti], propertyValues?[ti], false);
			}

			return list;
		}
	}
}
