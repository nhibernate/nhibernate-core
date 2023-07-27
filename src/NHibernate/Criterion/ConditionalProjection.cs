using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Defines a "switch" projection which supports multiple "cases" ("when/then's").
	/// </summary>
	/// <seealso cref="SimpleProjection" />
	/// <seealso cref="ConditionalProjectionCase" />
	[Serializable]
	public class ConditionalProjection : SimpleProjection
	{
		private readonly ConditionalProjectionCase[] _cases;
		private readonly IProjection _elseProjection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalProjection"/> class.
		/// </summary>
		/// <param name="criterion">The <see cref="ICriterion"/></param>
		/// <param name="whenTrue">The true <see cref="IProjection"/></param>
		/// <param name="whenFalse">The else <see cref="IProjection"/>.</param>
		public ConditionalProjection(ICriterion criterion, IProjection whenTrue, IProjection whenFalse)
		{
			_elseProjection = whenFalse;
			_cases = new[] {new ConditionalProjectionCase(criterion, whenTrue)};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalProjection"/> class.
		/// </summary>
		/// <param name="cases">The <see cref="ConditionalProjectionCase"/>s containing <see cref="ICriterion"/> and <see cref="IProjection"/> pairs.</param>
		/// <param name="elseProjection">The else <see cref="IProjection"/>.</param>
		public ConditionalProjection(ConditionalProjectionCase[] cases, IProjection elseProjection)
		{
			if (cases == null)
				throw new ArgumentNullException(nameof(cases));
			if (cases.Length == 0)
				throw new ArgumentException("Array should not be empty.", nameof(cases));

			_cases = cases;
			_elseProjection = elseProjection;
		}

		public override bool IsAggregate
		{
			get
			{
				if (_elseProjection.IsAggregate)
					return true;

				foreach (var projectionCase in _cases)
				{
					if (projectionCase.Projection.IsAggregate)
						return true;

					var projections = projectionCase.Criterion.GetProjections();
					if (projections != null)
					{
						foreach (var projection in projections)
						{
							if (projection.IsAggregate)
							{
								return true;
							}
						}
					}
				}

				return false;
			}
		}

		public override bool IsGrouped
		{
			get
			{
				if (_elseProjection.IsGrouped)
					return true;

				foreach (var projectionCase in _cases)
				{
					if (projectionCase.Projection.IsGrouped)
						return true;

					var projections = projectionCase.Criterion.GetProjections();
					if (projections != null)
					{
						foreach (var projection in projections)
						{
							if (projection.IsGrouped)
								return true;
						}
					}
				}

				return false;
			}
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			var sqlBuilder = new SqlStringBuilder(5 + (_cases.Length * 4));

			sqlBuilder.Add("(case");

			foreach (var projectionCase in _cases)
			{
				sqlBuilder.Add(" when ");
				sqlBuilder.Add(projectionCase.Criterion.ToSqlString(criteria, criteriaQuery));
				sqlBuilder.Add(" then ");
				sqlBuilder.AddObject(CriterionUtil.GetColumnNameAsSqlStringPart(projectionCase.Projection, criteriaQuery, criteria));
			}

			sqlBuilder.Add(" else ");
			sqlBuilder.AddObject(CriterionUtil.GetColumnNameAsSqlStringPart(_elseProjection, criteriaQuery, criteria));

			sqlBuilder.Add(" end) as ");
			sqlBuilder.Add(GetColumnAlias(position));

			return sqlBuilder.ToSqlString();
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var elseTypes = _elseProjection.GetTypes(criteria, criteriaQuery);

			for (var i = 0; i < _cases.Length; i++)
			{
				var subsequentTypes = _cases[i].Projection.GetTypes(criteria, criteriaQuery);
				if (!AreTypesEqual(elseTypes, subsequentTypes))
				{
					string msg = "All projections must return the same types." + Environment.NewLine +
					             "But Else projection returns: [" + string.Join<IType>(", ", elseTypes) + "] " + Environment.NewLine +
					             "And When projection " + i + " returns: [" + string.Join<IType>(", ", subsequentTypes) + "]";

					throw new HibernateException(msg);
				}
			}

			return elseTypes;
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var typedValues = new List<TypedValue>();
			
			foreach (var projectionCase in _cases)
			{
				typedValues.AddRange(projectionCase.Criterion.GetTypedValues(criteria, criteriaQuery));
				typedValues.AddRange(projectionCase.Projection.GetTypedValues(criteria, criteriaQuery));
			}

			typedValues.AddRange(_elseProjection.GetTypedValues(criteria, criteriaQuery));

			return typedValues.ToArray();
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var sqlBuilder = new SqlStringBuilder();

			foreach (var projection in _cases)
			{
				AddToGroupedSql(sqlBuilder, projection.Criterion.GetProjections(), criteria, criteriaQuery);
				AddToGroupedSql(sqlBuilder, projection.Projection, criteria, criteriaQuery);
			}

			AddToGroupedSql(sqlBuilder, _elseProjection, criteria, criteriaQuery);

			// Remove last comma 
			if (sqlBuilder.Count >= 2)
			{
				sqlBuilder.RemoveAt(sqlBuilder.Count - 1);
			}

			return sqlBuilder.ToSqlString();
		}

		private static bool AreTypesEqual(IType[] types1, IType[] types2)
		{
			bool areEqual = types1.Length == types2.Length;
			if (!areEqual)
			{
				return false;
			}

			for (int i = 0; i < types1.Length; i++)
			{
				if (types1[i].ReturnedClass != types2[i].ReturnedClass)
				{
					return false;
				}
			}

			return true;
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, IProjection[] projections, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projections == null) 
				return;
			
			foreach (var projection in projections)
			{
				AddToGroupedSql(sqlBuilder, projection, criteria, criteriaQuery);
			}
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, IProjection projection, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projection.IsGrouped)
			{
				sqlBuilder.Add(projection.ToGroupSqlString(criteria, criteriaQuery));
				sqlBuilder.Add(", ");
			}
		}
	}
}
