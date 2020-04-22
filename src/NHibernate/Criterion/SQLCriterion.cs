using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that creates a SQLExpression.
	/// The string {alias} will be replaced by the alias of the root entity.
	/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddCriteriaAliases("a", "bc")
	/// </summary>
	/// <remarks>
	/// This allows for database specific Expressions at the cost of needing to 
	/// write a correct <see cref="SqlString"/>.
	/// </remarks>
	[Serializable]
	public class SQLCriterion : AbstractCriterion
	{
		private readonly SqlString _sql;
		private readonly TypedValue[] _typedValues;
		private List<string> _criteriaAliases;

		public SQLCriterion(SqlString sql, object[] values, IType[] types)
		{
			_sql = sql;
			_typedValues = new TypedValue[values.Length];
			for (int i = 0; i < _typedValues.Length; i++)
			{
				_typedValues[i] = new TypedValue(types[i], values[i]);
			}
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var parameters = _sql.GetParameters().ToList();
			var paramPos = 0;
			for (int i = 0; i < _typedValues.Length; i++)
			{
				var controlledParameters = criteriaQuery.NewQueryParameter(_typedValues[i]);
				foreach (Parameter parameter in controlledParameters)
				{
					parameters[paramPos++].BackTrack = parameter.BackTrack;
				}
			}
			return SQLProjection.GetSqlString(criteria, criteriaQuery, _sql, _criteriaAliases);
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return _typedValues;
		}

		public override IProjection[] GetProjections()
		{
			return null;
		}

		public override string ToString()
		{
			return _sql.ToString();
		}

		/// <summary>
		/// Provide list of criteria aliases that's used in SQL projection.
		/// To be replaced with SQL aliases.
		/// </summary>
		public SQLCriterion AddCriteriaAliases(params string[] criteriaAliases)
		{
			if(_criteriaAliases == null)
				_criteriaAliases = new List<string>();

			_criteriaAliases.AddRange(criteriaAliases);

			return this;
		}
	}
}
