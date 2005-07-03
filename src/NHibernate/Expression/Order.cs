using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Expression
{
	/// <summary>
	/// Represents an order imposed upon a <see cref="ICriteria"/>
	/// result set.
	/// </summary>
	public class Order
	{
		private bool _ascending;
		private string _propertyName;

		/// <summary>
		/// Constructor for Order.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="ascending"></param>
		public Order( string propertyName, bool ascending )
		{
			_propertyName = propertyName;
			_ascending = ascending;
		}


		/// <summary>
		/// Render the SQL fragment
		/// </summary>
		/// <param name="sessionFactory"></param>
		/// <param name="persistentClass"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string ToStringForSql( ISessionFactoryImplementor sessionFactory, System.Type persistentClass, string alias )
		{
			string[ ] columns = AbstractCriterion.GetColumns( sessionFactory, persistentClass, _propertyName, alias, emptyMap );
			if( columns.Length != 1 )
			{
				throw new HibernateException( "Cannot order by multi-column property: " + _propertyName );
			}
			return columns[ 0 ] + ( _ascending ? " asc" : " desc" );
		}

		/// <summary>
		/// Ascending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Asc( string propertyName )
		{
			return new Order( propertyName, true );
		}

		/// <summary>
		/// Descending order
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static Order Desc( string propertyName )
		{
			return new Order( propertyName, false );
		}

		private static readonly IDictionary emptyMap = new Hashtable( 0 );
	}
}