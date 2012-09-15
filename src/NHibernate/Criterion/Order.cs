using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

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
			if (projection != null)
			{
				SqlString sb = SqlString.Empty;
				SqlString produced = this.projection.ToSqlString(criteria, 0, criteriaQuery, new Dictionary<string, IFilter>());
				SqlString truncated = SqlStringHelper.RemoveAsAliasesFromSql(produced);
				sb = sb.Append(truncated);
				sb = sb.Append(ascending ? " asc" : " desc");
				return sb;
			}

			string[] columns = criteriaQuery.GetColumnAliasesUsingProjection(criteria, propertyName);
			Type.IType type = criteriaQuery.GetTypeUsingProjection(criteria, propertyName);

			StringBuilder fragment = new StringBuilder();
			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			for (int i = 0; i < columns.Length; i++)
			{
				bool lower = ignoreCase && IsStringType(type.SqlTypes(factory)[i]);

				if (lower)
				{
					fragment.Append(factory.Dialect.LowercaseFunction)
						.Append("(");
				}
				fragment.Append(columns[i]);

				if (lower)
				{
					fragment.Append(")");
				}

				fragment.Append(ascending ? " asc" : " desc");

				if (i < columns.Length - 1)
				{
					fragment.Append(", ");
				}
			}

			return new SqlString(fragment.ToString());
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

			return new TypedValue[0]; // not using parameters for ORDER BY columns
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
