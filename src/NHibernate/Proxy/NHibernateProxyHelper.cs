using NHibernate.Cfg;
using NHibernate.Intercept;

namespace NHibernate.Proxy
{
	/// <summary>
	/// NHibernateProxyHelper provides convenience methods for working with
	/// objects that might be instances of Classes or the Proxied version of 
	/// the Class.
	/// </summary>
	public static class NHibernateProxyHelper
	{
		/// <summary> 
		/// Get the class of an instance or the underlying class of a proxy (without initializing the proxy!). 
		/// It is almost always better to use the entity name!
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public static System.Type GetClassWithoutInitializingProxy(object obj)
		{
			if (obj.IsProxy())
			{
				var proxy = obj as INHibernateProxy;

				return proxy.HibernateLazyInitializer.PersistentClass;
			}
			return obj.GetType();
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
			if (entity.IsProxy())
			{
				var proxy = entity as INHibernateProxy;
				var li = proxy.HibernateLazyInitializer;
				if (li.IsUninitialized)
				{
					return li.PersistentClass;
				}
				//NH-3145 : implementation could be a IFieldInterceptorAccessor 
				entity = li.GetImplementation();
			}
			var fieldInterceptorAccessor = entity as IFieldInterceptorAccessor;
			if (fieldInterceptorAccessor != null)
			{
				var fieldInterceptor = fieldInterceptorAccessor.FieldInterceptor;
				return fieldInterceptor.MappedClass;
			}
			return entity.GetType();
		}

		public static bool IsProxy(this object entity)
		{
			return Environment.BytecodeProvider.ProxyFactoryFactory.IsProxy(entity);
		}
	}
}

