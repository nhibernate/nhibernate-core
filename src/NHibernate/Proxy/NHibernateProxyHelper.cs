using Castle.Core.Interceptor;
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
		/// Gets the <see cref="AbstractLazyInitializer"/> that is used by the Proxy.
		/// </summary>
		/// <param name="proxy">The Proxy object</param>
		/// <returns>
		/// A reference to <see cref="AbstractLazyInitializer"/> that contains the details 
		/// of the Proxied object.
		/// </returns>
		public static ILazyInitializer GetLazyInitializer(INHibernateProxy proxy)
		{
			return proxy.HibernateLazyInitializer;
		}

		/// <summary>
		/// Convenience method to figure out the underlying type for the object regardless of it
		/// is a Proxied object or the real object.
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public static System.Type GetClass(object obj)
		{
			if (obj is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy) obj;
				ILazyInitializer li = GetLazyInitializer(proxy);
				return li.PersistentClass;
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
		/// <param name="proxy">a persistable object or proxy</param>
		/// <returns>guessed class of the instance</returns>
		/// <remarks>
		/// This method is approximate match for Session.bestGuessEntityName in H3.2
		/// </remarks>
		public static System.Type GuessClass(object proxy)
		{
			if (proxy is INHibernateProxy)
			{
				ILazyInitializer li = GetLazyInitializer((INHibernateProxy) proxy);
				if (li.IsUninitialized)
				{
					return li.PersistentClass;
				}
				else
				{
					return li.GetImplementation().GetType();
				}
			}
			else
			{
				return proxy.GetType();
			}
		}

		public static object GetIdentifier(object obj, IEntityPersister persister)
		{
			if (obj is INHibernateProxy)
			{
				INHibernateProxy proxy = (INHibernateProxy) obj;
				ILazyInitializer li = GetLazyInitializer(proxy);
				return li.Identifier;
			}
			else
			{
				return persister.GetIdentifier(obj);
			}
		}
	}
}
