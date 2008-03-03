namespace NHibernate.Expressions
{
	using System;
	using System.Collections.Generic;
	using Engine;
	using SqlCommand;
	using Type;
	using Util;

	public static class CriterionUtil
	{
		public static SqlString[] GetColumnNames(
			string propertyName,
			IProjection projection,
			ICriteriaQuery criteriaQuery,
			ICriteria criteria,
			IDictionary<string, IFilter> enabledFilters)
		{
			if (projection == null)
				return GetColumnNamesUsingPropertyName(criteriaQuery, criteria, propertyName);
			else
				return GetColumnNamesUsingProjection(projection, criteriaQuery, criteria, enabledFilters);
		}

		public static SqlString[] GetColumnNamesForSimpleExpression(
			string propertyName,
			IProjection projection,
			ICriteriaQuery criteriaQuery,
			ICriteria criteria,
			IDictionary<string, IFilter> enabledFilters,
			ICriterion criterion,
			object value)
		{
			if (projection == null)
			{
				return GetColumnNamesUsingPropertyName(
					criteriaQuery, 
					criteria, 
					propertyName,
					value, 
					criterion);
			}
			else
			{
				return GetColumnNamesUsingProjection(projection, criteriaQuery, criteria, enabledFilters);
			}
		}

		private static SqlString[] GetColumnNamesUsingProjection(IProjection projection, ICriteriaQuery criteriaQuery, ICriteria criteria,
																	 IDictionary<string, IFilter> enabledFilters)
		{
			SqlString sqlString = projection.ToSqlString(criteria, 
				criteriaQuery.GetIndexForAlias(),
				criteriaQuery, 
				enabledFilters);
			return new SqlString[]
				{
					StringHelper.RemoveAsAliasesFromSql(sqlString)
				};
		}

		private static SqlString[] GetColumnNamesUsingPropertyName(ICriteriaQuery criteriaQuery, ICriteria criteria, string propertyName)
		{
			string[] columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
			return Array.ConvertAll<string, SqlString>(columnNames, delegate(string input) { return new SqlString(input); });
		}

		private static SqlString[] GetColumnNamesUsingPropertyName(
			ICriteriaQuery criteriaQuery, 
			ICriteria criteria, 
			string propertyName, 
			object value, 
			ICriterion critertion)
		{
			string[] columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
			IType propertyType = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);

			if (value != null && !(value is System.Type) && !propertyType.ReturnedClass.IsInstanceOfType(value))
			{
				throw new QueryException(string.Format(
											"Type mismatch in {0}: {1} expected type {2}, actual type {3}",
											critertion.GetType(), propertyName, propertyType.ReturnedClass, value.GetType()));
			}

			if (propertyType.IsCollectionType)
			{
				throw new QueryException(string.Format(
											"cannot use collection property ({0}.{1}) directly in a criterion,"
											+ " use ICriteria.CreateCriteria instead",
											criteriaQuery.GetEntityName(criteria), propertyName));
			}
			return Array.ConvertAll<string, SqlString>(columnNames, delegate(string col)
			{
				return new SqlString(col);
			});
		}

		public static TypedValue[] GetTypedValues(ICriteriaQuery criteriaQuery, ICriteria criteria,
		                                          IProjection projection,
		                                          string propertyName,
		                                          params object[] values)
		{
			List<TypedValue> types = new List<TypedValue>();
			if (projection == null)
			{
				foreach (object value in values)
				{
					TypedValue typedValue = criteriaQuery.GetTypedValue(criteria, propertyName, value); 					
					types.Add(typedValue);
				}
			}
			else
			{
				TypedValue[] typedValues = projection.GetTypedValues(criteria, criteriaQuery);
				types.AddRange(typedValues);
				// we add the values collection _twice_ on purpose.
				// once is for the projection types, if they exists
				// the second for the parameters of the expression itself
				// if the projection is using no types, we will try to guess
				// what we have here
				foreach (object value in values)
				{
					if (typedValues.Length != 0)
					{
						types.AddRange(typedValues);
					}
					else
					{
						types.Add(new TypedValue(NHibernateUtil.GuessType((object)value), value, EntityMode.Poco));
					}
				}

			}
			return types.ToArray();

		}
	}
}