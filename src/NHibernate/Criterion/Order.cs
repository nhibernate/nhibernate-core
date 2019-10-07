using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Represents an order imposed upon a <see cref="ICriteria"/>
	/// result set.
	/// </summary>
	/// <remarks>
	/// Should Order implement ICriteriaQuery?
	/// </remarks>
	[Serializable]
	public class Order
	{
		protected bool ascending;
		protected string propertyName;
		protected IProjection projection;
		private bool ignoreCase;

		public Order(IProjection projection, bool ascending)
		{
			this.projection = projection;
			this.ascending = ascending;
		}

		public Order(string propertyName, bool ascending)
		{
			this.propertyName = propertyName;
			this.ascending = ascending;
		}

		/// <summary>
		/// Render the SQL fragment
		/// </summary>
		public virtual SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var columnsOrAliases = GetColumnsOrAliases(criteria, criteriaQuery);
			var sqlTypes = ignoreCase ? SqlTypes(criteria, criteriaQuery) : null;

			var fragment = new SqlStringBuilder();
			var factory = criteriaQuery.Factory;
			for (var i = 0; i < columnsOrAliases.Length; i++)
			{
				var lower = sqlTypes != null && IsStringType(sqlTypes[i]);
				if (lower)
				{
					fragment
						.Add(factory.Dialect.LowercaseFunction)
						.Add("(");
				}

				fragment.AddObject(columnsOrAliases[i]);

				if (lower)
				{
					fragment.Add(")");
				}

				fragment.Add(ascending ? " asc" : " desc");

				if (i < columnsOrAliases.Length - 1)
				{
					fragment.Add(", ");
				}
			}

			return fragment.ToSqlString();
		}

		private object[] GetColumnsOrAliases(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var propName = propertyName ?? (projection as IPropertyProjection)?.PropertyName;
			return propName != null
				? criteriaQuery.GetColumnAliasesUsingProjection(criteria, propName)
				: (object[]) CriterionUtil.GetColumnNamesUsingProjection(projection, criteriaQuery, criteria);
		}

		private SqlType[] SqlTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var type = projection == null
				? criteriaQuery.GetTypeUsingProjection(criteria, propertyName)
				: projection.GetTypes(criteria, criteriaQuery)[0];

			return type.SqlTypes(criteriaQuery.Factory);
		}

		public override string ToString()
		{
			return (projection != null ? projection.ToString() : propertyName) + (ascending ? " asc" : " desc");
		}

		/// <summary>
		/// Ascending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Asc(string propertyName)
		{
			return new Order(propertyName, true);
		}

		/// <summary>
		/// Ascending order
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static Order Asc(IProjection projection)
		{
			return new Order(projection, true);
		}

		/// <summary>
		/// Descending order
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static Order Desc(IProjection projection)
		{
			return new Order(projection, false);
		}

		/// <summary>
		/// Descending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Desc(string propertyName)
		{
			return new Order(propertyName, false);
		}

		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projection != null)
				return projection.GetTypedValues(criteria, criteriaQuery);

			return Array.Empty<TypedValue>(); // not using parameters for ORDER BY columns
		}

		public Order IgnoreCase()
		{
			ignoreCase = true;
			return this;
		}

		private bool IsStringType(SqlTypes.SqlType propertyType)
		{
			switch (propertyType.DbType)
			{
				case System.Data.DbType.AnsiString:
					return true;
				case System.Data.DbType.AnsiStringFixedLength:
					return true;
				case System.Data.DbType.String:
					return true;
				case System.Data.DbType.StringFixedLength:
					return true;
				default:
					return false;
			}
		}
	}
}
