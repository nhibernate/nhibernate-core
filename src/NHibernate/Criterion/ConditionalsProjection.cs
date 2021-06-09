using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Defines a "switch" projection which supports multiple "cases" ("when/then's") using <see cref="ConditionalCriterionProjectionPair"/>.
	/// Can be used in Orderby for example.
	/// </summary>
	/// <seealso cref="SimpleProjection" />
	/// <seealso cref="ConditionalCriterionProjectionPair" />
	public sealed class ConditionalsProjection : SimpleProjection
	{
		private readonly ConditionalCriterionProjectionPair[] criterionProjections;
		private readonly IProjection elseProjection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalsProjection"/> class.
		/// </summary>
		/// <param name="conditionalProjections">The <see cref="ConditionalCriterionProjectionPair"/>s containg your <see cref="ICriterion"/> and <see cref="IProjection"/> pairs.</param>
		/// <param name="elseProjection">The else <see cref="IProjection"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="conditionalProjections"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="elseProjection"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="conditionalProjections"/> should not be empty.</exception>
		public ConditionalsProjection(ConditionalCriterionProjectionPair[] conditionalProjections, IProjection elseProjection)
		{
			if (elseProjection is null)
			{
				throw new ArgumentNullException(nameof(elseProjection));
			}

			if (conditionalProjections is null)
			{
				throw new ArgumentNullException(nameof(conditionalProjections));
			}

			if (conditionalProjections.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(this.criterionProjections), "Array should not be empty.");
			}

			this.criterionProjections = conditionalProjections.ToArray();
			this.elseProjection = elseProjection;
		}

		public override bool IsAggregate
		{
			get
			{
				if (this.elseProjection.IsAggregate)
					return true;

				for (int i = 0; i < this.criterionProjections.Length; i++)
				{
					if (this.CalcIsAggregate(this.criterionProjections[i]))
						return true;
				}

				return false;
			}
		}

		public override bool IsGrouped => this.elseProjection.IsGrouped || this.CalcIsGrouped(this.criterionProjections);

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			object[] parts = new object[5 + (this.criterionProjections.Length * 4)];
			var index = 0;

			parts[index++] = "(case";

			for (int i = 0; i < criterionProjections.Length; i++)
			{
				ConditionalCriterionProjectionPair conditional = this.criterionProjections[i];
				parts[index++] = " when ";
				parts[index++] = conditional.Criterion.ToSqlString(criteria, criteriaQuery);
				parts[index++] = " then ";
				parts[index++] = CriterionUtil.GetColumnNameAsSqlStringPart(conditional.Projection, criteriaQuery, criteria);
			}

			parts[index++] = " else ";
			parts[index++] = CriterionUtil.GetColumnNameAsSqlStringPart(this.elseProjection, criteriaQuery, criteria);

			parts[index++] = " end) as ";
			parts[index++] = this.GetColumnAlias(position);

			return new SqlString(parts);
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var elseTypes = this.elseProjection.GetTypes(criteria, criteriaQuery);

			for (int i = 0; i < this.criterionProjections.Length; i++)
			{
				var subsequentTypes = this.criterionProjections[i].Projection.GetTypes(criteria, criteriaQuery);

				this.AssertAreEqualTypes(elseTypes, subsequentTypes, i.ToString());
			}

			return elseTypes;
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> typedValues = new List<TypedValue>();

			for (int i = 0; i < this.criterionProjections.Length; i++)
			{
				typedValues.AddRange(this.criterionProjections[i].Criterion.GetTypedValues(criteria, criteriaQuery));
				typedValues.AddRange(this.criterionProjections[i].Projection.GetTypedValues(criteria, criteriaQuery));
			}

			typedValues.AddRange(this.elseProjection.GetTypedValues(criteria, criteriaQuery));

			return typedValues.ToArray();
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			this.AddToGroupedSql(sqlBuilder, this.criterionProjections, criteria, criteriaQuery);
			this.AddToGroupedSql(sqlBuilder, this.elseProjection, criteria, criteriaQuery);

			// ?? 
			if (sqlBuilder.Count >= 2)
			{
				sqlBuilder.RemoveAt(sqlBuilder.Count - 1);
			}

			return sqlBuilder.ToSqlString();
		}

		private bool CalcIsGrouped(IList<ConditionalCriterionProjectionPair> criterionProjections)
		{
			for (int i = 0; i < criterionProjections.Count; i++)
			{
				if (criterionProjections[i].Projection.IsGrouped)
					return true;

				if (this.CalcIsGrouped(criterionProjections[i].Criterion.GetProjections()))
					return true;
			}

			return false;
		}

		private void AssertAreEqualTypes(IType[] types1, IType[] types2, string projectionPosName)
		{
			bool areEqual = types1.Length == types2.Length;
			if (areEqual)
			{
				for (int i = 0; i < types1.Length; i++)
				{
					if (!types1[i].ReturnedClass.Equals(types2[i].ReturnedClass))
					{
						areEqual = false;
						break;
					}
				}
			}

			if (!areEqual)
			{
				string msg = "All projections must return the same types." + Environment.NewLine +
							 "But First projection returns: [" + string.Join<IType>(", ", types1) + "] " + Environment.NewLine +
							 "And projection " + projectionPosName + " returns: [" + string.Join<IType>(", ", types2) + "]";

				throw new HibernateException(msg);
			}
		}

		private bool CalcIsGrouped(IProjection[] projections)
		{
			for (int j = 0; j < projections.Length; j++)
			{
				if (projections[j].IsGrouped)
					return true;
			}

			return false;
		}

		private bool CalcIsAggregate(ConditionalCriterionProjectionPair criterionProjection)
		{
			if (criterionProjection.Projection.IsAggregate)
				return true;

			var projections = criterionProjection.Criterion.GetProjections();
			for (int i = 0; i < projections.Length; i++)
			{
				if (projections[i].IsAggregate)
					return true;
			}

			return false;
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, ConditionalCriterionProjectionPair[] criterionProjections, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			for (int i = 0; i < criterionProjections.Length; i++)
			{
				this.AddToGroupedSql(sqlBuilder, criterionProjections[i], criteria, criteriaQuery);
			}
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, ConditionalCriterionProjectionPair criterionProjection, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			this.AddToGroupedSql(sqlBuilder, criterionProjection.Criterion, criteria, criteriaQuery);
			this.AddToGroupedSql(sqlBuilder, criterionProjection.Projection, criteria, criteriaQuery);
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, ICriterion criterion, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			this.AddToGroupedSql(sqlBuilder, criterion.GetProjections(), criteria, criteriaQuery);
		}

		private void AddToGroupedSql(SqlStringBuilder sqlBuilder, IProjection[] projections, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projections is object)
			{
				for (int i = 0; i < projections.Length; i++)
				{
					this.AddToGroupedSql(sqlBuilder, projections[i], criteria, criteriaQuery);
				}
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
