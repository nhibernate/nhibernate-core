namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using Util;

	/// <summary>
	/// Superclass for an <see cref="ICriterion"/> that represents a
	/// constraint between two properties (with SQL binary operators).
	/// </summary>
	[Serializable]
	public abstract class PropertyExpression : AbstractCriterion
	{
		private static readonly TypedValue[] NoTypedValues = Array.Empty<TypedValue>();
		private readonly string _lhsPropertyName;
		private readonly string _rhsPropertyName;
		private readonly IProjection _lhsProjection;
		private readonly IProjection _rhsProjection;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The projection.</param>
		/// <param name="rhsPropertyName">Name of the RHS property.</param>
		protected PropertyExpression(IProjection lhsProjection, string rhsPropertyName)
		{
			this._lhsProjection = lhsProjection;
			_rhsPropertyName = rhsPropertyName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		protected PropertyExpression(IProjection lhsProjection, IProjection rhsProjection)
		{
			this._lhsProjection = lhsProjection;
			this._rhsProjection = rhsProjection;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsPropertyName">Name of the LHS property.</param>
		/// <param name="rhsPropertyName">Name of the RHS property.</param>
		protected PropertyExpression(string lhsPropertyName, string rhsPropertyName)
		{
			this._lhsPropertyName = lhsPropertyName;
			this._rhsPropertyName = rhsPropertyName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsPropertyName">Name of the LHS property.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		protected PropertyExpression(string lhsPropertyName, IProjection rhsProjection)
		{
			this._lhsPropertyName = lhsPropertyName;
			this._rhsProjection = rhsProjection;
		}

		/// <summary>
		/// Get the Sql operator to use for the property expression.
		/// </summary>
		protected abstract string Op { get; }

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var columnNames =
				CriterionUtil.GetColumnNamesAsSqlStringParts(_lhsPropertyName, _lhsProjection, criteriaQuery, criteria);
			var otherColumnNames =
				CriterionUtil.GetColumnNamesAsSqlStringParts(_rhsPropertyName, _rhsProjection, criteriaQuery, criteria);

			switch (columnNames.Length)
			{
				case 1:
					return new SqlString(columnNames[0], Op, otherColumnNames[0]);
				case 0:
					return SqlString.Empty;
			}

			var sb = new SqlStringBuilder();
			sb.Add(StringHelper.OpenParen);
			sb.AddObject(columnNames[0]).Add(Op).AddObject(otherColumnNames[0]);
			for (var i = 1; i < columnNames.Length; i++)
			{
				sb.Add(" and ");
				sb.AddObject(columnNames[i]).Add(Op).AddObject(otherColumnNames[i]);
			}
			sb.Add(StringHelper.ClosedParen);

			return sb.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (_lhsProjection == null && _rhsProjection == null)
			{
				return NoTypedValues;
			}
			else
			{
				List<TypedValue> types = new List<TypedValue>();
				if(_lhsProjection!=null)
				{
					types.AddRange(_lhsProjection.GetTypedValues(criteria, criteriaQuery));
				}
				if (_rhsProjection != null)
				{
					types.AddRange(_rhsProjection.GetTypedValues(criteria, criteriaQuery));
				}
				return types.ToArray();
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (_lhsProjection ?? (object)_lhsPropertyName) + Op + (_rhsProjection ?? (object)_rhsPropertyName);
		}

		public override IProjection[] GetProjections()
		{
			if(_lhsProjection != null && _rhsProjection != null)
			{
				return new IProjection[] {_lhsProjection, _rhsProjection};
			}
			if(_lhsProjection != null)
			{
				return new IProjection[] {_lhsProjection};
			}
			if(_rhsProjection != null)
			{
				return new IProjection[] {_rhsProjection};
			}
			return null;
		}
	}
}
