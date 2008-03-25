using Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	/// <summary> Helper class for dealing with enhanced entity classes. </summary>
	public static class FieldInterceptionHelper
	{
		// VERY IMPORTANT!!!! - This class needs to be free of any static references
		// to any Castle classes.  Otherwise, users will always need both
		// on their classpaths no matter which (if either) they use.
		//
		// Another option here would be to remove the Hibernate.isPropertyInitialized()
		// method and have the users go through the SessionFactory to get this information.
		
		
		public static bool IsInstrumented(System.Type entityClass)
		{
			// TODO : Here code
			return false;
		}
		
		public static bool IsInstrumented(object entity)
		{
			return entity != null && IsInstrumented(entity.GetType());
		}
		
		public static IFieldInterceptor ExtractFieldInterceptor(object entity)
		{
			if (entity == null)
			{
				return null;
			}
			// TODO : Here code to extract the Field Interceptor
			return null;
		}
		
		public static IFieldInterceptor InjectFieldInterceptor(object entity, string entityName, ISet<string> uninitializedFieldNames, ISessionImplementor session)
		{
			if (entity != null)
			{
				// TODO : Here code to inject the Field Interceptor
			}
			return null;
		}
		
		public static void  ClearDirty(object entity)
		{
			IFieldInterceptor interceptor = ExtractFieldInterceptor(entity);
			if (interceptor != null)
			{
				interceptor.ClearDirty();
			}
		}
		
		public static void  MarkDirty(object entity)
		{
			IFieldInterceptor interceptor = ExtractFieldInterceptor(entity);
			if (interceptor != null)
			{
				interceptor.MarkDirty();
			}
		}
	}
}