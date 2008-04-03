namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;

	/// <summary>
	/// An <see cref="ICriterion"/> that represents a "between" constraint.
	/// </summary>
	[Serializable]
	public class BetweenExpression : AbstractCriterion
	{
		private readonly object _hi;
		private readonly object _lo;
		private readonly IProjection _projection;

		private readonly string _propertyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="BetweenExpression"/> class.
		/// </summary>
		/// <param name="_projection">The _projection.</param>
		/// <param name="_lo">The _lo.</param>
		/// <param name="_hi">The _hi.</param>
		public BetweenExpression(IProjection _projection, object _lo, object _hi)
		{
			this._projection = _projection;
			this._lo = _lo;
			this._hi = _hi;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="BetweenExpression" /> class for
		/// the named Property.
		/// </summary>
		/// <param name="propertyName">The name of the Property of the Class.</param>
		/// <param name="lo">The low value for the BetweenExpression.</param>
		/// <param name="hi">The high value for the BetweenExpression.</param>
		public BetweenExpression(string propertyName, object lo, object hi)
		{
			_propertyName = propertyName;
			_lo = lo;
			_hi = hi;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                                      IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: add a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			//IType propertyType = criteriaQuery.GetTypeUsingProjection( criteria, _propertyName );
			SqlString[] columnNames =
				CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria, enabledFilters);

			criteriaQuery.AddUsedTypedValues(GetTypedValues(criteria, criteriaQuery));
			if (columnNames.Length == 1)
			{
				sqlBuilder
					.Add(columnNames[0])
					.Add(" between ")
					.AddParameter()
					.Add(" and ")
					.AddParameter();
			}
			else
			{
				bool andNeeded = false;

				for (int i = 0; i < columnNames.Length; i++)
				{
					if (andNeeded)
					{
						sqlBuilder.Add(" AND ");
					}
					andNeeded = true;

					sqlBuilder.Add(columnNames[i])
						.Add(" >= ")
						.AddParameter();
				}

				for (int i = 0; i < columnNames.Length; i++)
				{
					sqlBuilder.Add(" AND ")
						.Add(columnNames[i])
						.Add(" <= ")
						.AddParameter();
				}
			}

			return sqlBuilder.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return CriterionUtil.GetTypedValues(criteriaQuery, criteria, _projection, _propertyName, _lo, _hi);
		}

		public override IProjection[] GetProjections()
		{
			if(_projection != null)
			{
				return new IProjection[] { _projection };
			}
			return null;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " between " + _lo + " and " + _hi;
		}
	}
}
