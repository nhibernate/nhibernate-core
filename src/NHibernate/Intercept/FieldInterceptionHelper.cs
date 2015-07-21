using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	/// <summary> Helper class for dealing with enhanced entity classes. </summary>
	public static class FieldInterceptionHelper
	{
		// VERY IMPORTANT!!!! - This class needs to be free of any static references
		// to any Castle/Spring/LinFu classes.  Otherwise, users will always need both
		// on their classpaths no matter which (if either) they use.
		//
		// Another option here would be to remove the Hibernate.isPropertyInitialized()
		// method and have the users go through the SessionFactory to get this information.

		public static bool IsInstrumented(System.Type entityClass)
		{
			// NH Specific:
			// unlike Hibernate, NHibernate assumes that any class is valid for interception
			// because we don't try to handle field interception
			return Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.IsInstrumented(entityClass);
		}

		public static bool IsInstrumented(object entity)
		{
			return entity is IFieldInterceptorAccessor;
		}

		public static IFieldInterceptor ExtractFieldInterceptor(object entity)
		{
			var fieldInterceptorAccessor = entity as IFieldInterceptorAccessor;
			return fieldInterceptorAccessor == null ? null : fieldInterceptorAccessor.FieldInterceptor;
		}

		public static IFieldInterceptor InjectFieldInterceptor(object entity, string entityName, 
			System.Type mappedClass,
			ISet<string> uninitializedFieldNames, 
			ISet<string> unwrapProxyFieldNames,
			ISessionImplementor session)
		{
			var fieldInterceptorAccessor = entity as IFieldInterceptorAccessor;
			if (fieldInterceptorAccessor != null)
			{
				var fieldInterceptorImpl = new DefaultFieldInterceptor(session, uninitializedFieldNames, unwrapProxyFieldNames, entityName, mappedClass);
				fieldInterceptorAccessor.FieldInterceptor = fieldInterceptorImpl;
				return fieldInterceptorImpl;
			}
			return null;
		}

		public static void ClearDirty(object entity)
		{
			IFieldInterceptor interceptor = ExtractFieldInterceptor(entity);
			if (interceptor != null)
			{
				interceptor.ClearDirty();
			}
		}

		public static void MarkDirty(object entity)
		{
			IFieldInterceptor interceptor = ExtractFieldInterceptor(entity);
			if (interceptor != null)
			{
				interceptor.MarkDirty();
			}
		}
	}
}