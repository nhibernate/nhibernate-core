using NHibernate.Intercept;
using NHibernate.Persister.Entity;

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
		/// Get the class of an instance or the underlying class of a proxy (without initializing the proxy!). 
		/// It is almost always better to use the entity name!
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public static System.Type GetClassWithoutInitializingProxy(object obj)
		{
			INHibernateProxy proxy = obj as INHibernateProxy;
			if (proxy != null)
			{
				return proxy.HibernateLazyInitializer.PersistentClass;
			}
			else
			{
				return obj.GetType();
			}
		}

		/// <summary>
		/// Get the true, underlying class of a proxied persistent class. This operation
		/// will NOT initialize the proxy and thus may return an incorrect result.
		/// </summary>
		/// <param name="entity">a persistable object or proxy</param>
		/// <returns>guessed class of the instance</returns>
		/// <remarks>
		/// This method is approximate match for Session.bestGuessEntityName in H3.2
		/// </remarks>
		public static System.Type GuessClass(object entity)
		{
			var proxy = entity as INHibernateProxy;
			if (proxy != null)
			{
				ILazyInitializer li = proxy.HibernateLazyInitializer;
				if (li.IsUninitialized)
				{
					return li.PersistentClass;
				}
				return li.GetImplementation().GetType();
			}
			var fieldInterceptorAccessor = entity as IFieldInterceptorAccessor;
			if (fieldInterceptorAccessor != null)
			{
				var fieldInterceptor = fieldInterceptorAccessor.FieldInterceptor;
				return fieldInterceptor.MappedClass;
			}
			return entity.GetType();
		}
	}
}
