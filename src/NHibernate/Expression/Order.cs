using System;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Expression
{
	/// <summary>
	/// Represents an order imposed upon a <see cref="ICriteria"/>
	/// result set.
	/// </summary>
	[Serializable]
	public class Order
	{
		private bool _ascending;
		private string _propertyName;

		/// <summary>
		/// Constructor for Order.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="ascending"></param>
		public Order(string propertyName, bool ascending)
		{
			_propertyName = propertyName;
			_ascending = ascending;
		}


		/// <summary>
		/// Render the SQL fragment
		/// </summary>
		public string ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			string[] columns = criteriaQuery.GetPropertyColumnAliases(criteria, _propertyName);

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

				fragment.Append(_ascending ? " asc" : " desc");

				if (i < columns.Length - 1)
				{
					fragment.Append(", ");
				}
			}

			return fragment.ToString();
		}

		public override string ToString()
		{
			return _propertyName + (_ascending ? " asc" : " desc");
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