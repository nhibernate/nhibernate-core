using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents a "between" constraint.
	/// </summary>
	[Serializable]
	public class BetweenExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object _lo;
		private readonly object _hi;

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

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary enabledFilters)
		{
			//TODO: add a default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			//IType propertyType = criteriaQuery.GetTypeUsingProjection( criteria, _propertyName );
			string[] columnNames = criteriaQuery.GetPropertyColumnNames(criteria, _propertyName);

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
			return new TypedValue[]
				{
					criteriaQuery.GetTypedValue(criteria, _propertyName, _lo),
					criteriaQuery.GetTypedValue(criteria, _propertyName, _hi)
				};
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _propertyName + " between " + _lo + " and " + _hi;
		}
	}
}