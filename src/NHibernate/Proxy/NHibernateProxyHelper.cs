namespace NHibernate.Proxy
{
	/// <summary>
	/// NHibernateProxyHelper provides convenience methods for working with
	/// objects that might be instances of Classes or the Proxied version of 
	/// the Class.
	/// </summary>
	public sealed class NHibernateProxyHelper
	{
		private NHibernateProxyHelper()
		{
			//can't instantiate
		}

		/// <summary>
		/// Gets the <see cref="LazyInitializer"/> that is used by the Proxy.
		/// </summary>
		/// <param name="proxy">The Proxy object</param>
		/// <returns>
		/// A reference to <see cref="LazyInitializer"/> that contains the details 
		/// of the Proxied object.
		/// </returns>
		public static LazyInitializer GetLazyInitializer( INHibernateProxy proxy )
		{
			LazyInitializer li = ProxyGeneratorFactory.GetProxyGenerator().GetLazyInitializer( proxy );
			return li;

		}

		/// <summary>
		/// Convenience method to figure out the underlying type for the object regardless of it
		/// is a Proxied object or the real object.
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public static System.Type GetClass( object obj )
		{
			if( obj is INHibernateProxy )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) obj;
				LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer( proxy );
				return li.PersistentClass;
			}
			else
			{
				return obj.GetType();
			}
		}
	}
}