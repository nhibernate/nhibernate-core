using System;
using System.Collections;
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
	/// <remarks>
	/// InExpression - should only be used with a Single Value column - no multicolumn properties...
	/// </remarks>
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

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
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

			//TODO: add default capacity
			SqlStringBuilder result = new SqlStringBuilder();
			SqlString[] columnNames =
				CriterionUtil.GetColumnNames(_propertyName, _projection, criteriaQuery, criteria, enabledFilters);
			
			// Generate SqlString of the form:
			// columnName1 in (values) and columnName2 in (values) and ...
			Parameter[] parameters = GetParameterTypedValues(criteria, criteriaQuery).SelectMany(t => criteriaQuery.NewQueryParameter(t)).ToArray();

			for (int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
			{
				SqlString columnName = columnNames[columnIndex];

				if (columnIndex > 0)
				{
					result.Add(" and ");
				}

				result
					.Add(columnName)
					.Add(" in (");

				for (int i = 0; i < _values.Length; i++)
				{
					if (i > 0)
					{
						result.Add(StringHelper.CommaSpace);
					}
					result.Add(parameters[i]);
				}

				result.Add(")");
			}

			return result.ToSqlString();
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

			if (type.IsComponentType)
			{
				List<TypedValue> list = new List<TypedValue>();
				IAbstractComponentType actype = (IAbstractComponentType) type;
				IType[] types = actype.Subtypes;

				for (int i = 0; i < types.Length; i++)
				{
					for (int j = 0; j < _values.Length; j++)
					{
						object subval = _values[j] == null
											? null
											: actype.GetPropertyValues(_values[j], EntityMode.Poco)[i];
						list.Add(new TypedValue(types[i], subval, EntityMode.Poco));
					}
				}

				return list;
			}
			else
			{
				return _values.Select(v => new TypedValue(type, v, EntityMode.Poco)).ToList();
			}
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
