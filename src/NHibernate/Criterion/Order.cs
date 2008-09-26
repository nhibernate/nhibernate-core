using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Represents an order imposed upon a <see cref="ICriteria"/>
	/// result set.
	/// </summary>
	[Serializable]
	public class Order
	{
		protected bool ascending;
		protected string propertyName;
		protected IProjection projection;
		/// <summary>
		/// Constructor for Order.
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="ascending"></param>
		public Order(IProjection projection, bool ascending)
		{
			this.projection = projection;
			this.ascending = ascending;
		}

		/// <summary>
		/// Constructor for Order.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="ascending"></param>
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
			if(projection!=null)
			{
				SqlString sb=new SqlString();
				SqlString produced = this.projection.ToSqlString(criteria, 0, criteriaQuery, new Dictionary<string, IFilter>());
				SqlString truncated = NHibernate.Util.StringHelper.RemoveAsAliasesFromSql(produced);
				sb = sb.Append(truncated);
				sb = sb.Append(ascending ? " asc" : " desc");
				return sb;
			}

			string[] columns = criteriaQuery.GetColumnAliasesUsingProjection(criteria, propertyName);

			StringBuilder fragment = new StringBuilder();

			ISessionFactoryImplementor factory = criteriaQuery.Factory;
			for (int i = 0; i < columns.Length; i++)
			{
				// TODO H3: bool lower = _ignoreCase && type.SqlTypes( factory )[ i ] == Types.VARCHAR
				bool lower = false;
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
			return (projection!=null?projection.ToString():propertyName) + (ascending ? " asc" : " desc");
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
	}
}
