using System;

using NHibernate.Engine;

namespace NHibernate.Expression {

	/// <summary>
	/// Represents an order imposed upon a ICriteria result set
	/// </summary>
	public class Order {

		private bool ascending;
		private string propertyName;

		/// <summary>
		/// Constructor for Order.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="ascending"></param>
		protected Order(string propertyName, bool ascending) {
			this.propertyName = propertyName;
			this.ascending = ascending;
		}
	
		/// <summary>
		/// Render the SQL fragment
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string ToSqlString(ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias) {
			string[] columns = Expression.GetColumns(sessionFactory, persistentClass, propertyName, alias);
			if (columns.Length!=1) throw new HibernateException("Cannot order by multi-column property: " + propertyName);
			return columns[0] + ( ascending ? " asc" : " desc" );
		}
	
		/// <summary>
		/// Ascending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Asc(String propertyName) {
			return new Order(propertyName, true);
		}
	
		/// <summary>
		/// Descending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Desc(String propertyName) {
			return new Order(propertyName, false);
		}
	}
}