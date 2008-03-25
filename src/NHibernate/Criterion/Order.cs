using System;
using System.Text;
using NHibernate.Engine;

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
		public virtual string ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
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
						.Append('(');
				}
				fragment.Append(columns[i]);

				if (lower)
				{
					fragment.Append(')');
				}

				fragment.Append(ascending ? " asc" : " desc");

				if (i < columns.Length - 1)
				{
					fragment.Append(", ");
				}
			}

			return fragment.ToString();
		}

		public override string ToString()
		{
			return propertyName + (ascending ? " asc" : " desc");
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