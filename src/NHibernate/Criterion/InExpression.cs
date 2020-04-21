using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that constrains the property
	/// to a specified list of values.
	/// </summary>
	[Serializable]
	public class InExpression : AbstractCriterion
	{
		private readonly IProjection _projection;
		private readonly string _propertyName;
		private object[] _values;

		/// <summary>
		/// Initializes a new instance of the <see cref="InExpression"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="values">The _values.</param>
		public InExpression(IProjection projection, object[] values)
		{
			_projection = projection;
			_values = values;
		}

		public InExpression(string propertyName, object[] values)
		{
			_propertyName = propertyName;
			_values = values;
		}

		public override IProjection[] GetProjections()
		{
			if (_projection != null)
			{
				return new IProjection[] { _projection };
			}
			return null;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (_projection == null)
			{
				AssertPropertyIsNotCollection(criteriaQuery, criteria);
			}

			if (_values.Length == 0)
			{
				// "something in ()" is always false
				return new SqlString("1=0");
			}

			SqlString[] columns = CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria);

			var list = new List<Parameter>(columns.Length * Values.Length);
			foreach (var typedValue in GetParameterTypedValues(criteria, criteriaQuery))
			{
				//Must be executed after CriterionUtil.GetColumnNames (as it might add _projection parameters to criteria)
				list.AddRange(criteriaQuery.NewQueryParameter(typedValue));
			}

			var bogusParam = Parameter.Placeholder;

			var sqlString = GetSqlString(criteriaQuery, columns, bogusParam);
			sqlString.SubstituteBogusParameters(list, bogusParam);
			return sqlString;
		}

		private SqlString GetSqlString(ICriteriaQuery criteriaQuery, SqlString[] columns, Parameter bogusParam)
		{
			if (columns.Length <= 1 || criteriaQuery.Factory.Dialect.SupportsRowValueConstructorSyntaxInInList)
			{
				var wrapInParens = columns.Length > 1;
				const string comaSeparator = ", ";
				var singleValueParam = SqlStringHelper.Repeat(new SqlString(bogusParam), columns.Length, comaSeparator, wrapInParens);

				var parameters = SqlStringHelper.Repeat(singleValueParam, Values.Length, comaSeparator,  wrapInParens: false);

				//single column: col1 in (?, ?)
				//multi column:  (col1, col2) in ((?, ?), (?, ?))
				return new SqlString(
					wrapInParens ? StringHelper.OpenParen : string.Empty,
					SqlStringHelper.Join(comaSeparator, columns),
					wrapInParens ? StringHelper.ClosedParen : string.Empty,
					" in (",
					parameters,
					")");
			}

			//((col1 = ? and col2 = ?) or (col1 = ? and col2 = ?))
			var cols = new SqlString(
				" ( ",
				SqlStringHelper.Join(new SqlString(" = ", bogusParam, " and "), columns),
				"= ",
				bogusParam,
				" ) ");
			cols = SqlStringHelper.Repeat(cols, Values.Length, " or ", wrapInParens: Values.Length > 1);
			return cols;
		}

		private void AssertPropertyIsNotCollection(ICriteriaQuery criteriaQuery, ICriteria criteria)
		{
			IType type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);
			if (type.IsCollectionType)
			{
				throw new QueryException("Cannot use collections with InExpression");
			}
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var list = GetParameterTypedValues(criteria, criteriaQuery);

			if (_projection != null)
				list.InsertRange(0, _projection.GetTypedValues(criteria, criteriaQuery));

			return list.ToArray();
		}

		private List<TypedValue> GetParameterTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType type = GetElementType(criteria, criteriaQuery);

			if (!type.IsComponentType)
			{
				return _values.ToList(v => new TypedValue(type, v, false));
			}

			List<TypedValue> list = new List<TypedValue>();
			IAbstractComponentType actype = (IAbstractComponentType) type;
			var types = actype.Subtypes;
			foreach (var value in _values)
			{
				var propertyValues = value != null ? actype.GetPropertyValues(value) : null;
				for (int ti = 0; ti < types.Length; ti++)
				{
					list.Add(new TypedValue(types[ti], propertyValues?[ti], false));
				}
			}

			return list;
		}

		/// <summary>
		/// Determine the type of the elements in the IN clause.
		/// </summary>
		private IType GetElementType(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (_projection == null)
				return criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);

			IType[] types = _projection.GetTypes(criteria, criteriaQuery);
			if (types.Length != 1)
				throw new QueryException("Cannot use projections that return more than a single column with InExpression");

			return types[0];
		}

		public object[] Values
		{
			get { return _values; }
			protected set { _values = value; }
		}

		public override string ToString()
		{
			return (_projection ?? (object)_propertyName) + " in (" + StringHelper.ToString(_values) + ')';
		}
	}
}
