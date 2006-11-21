using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Hql.Util
{
	/// <summary>
	/// Wraps SessionFactoryImpl, adding more lookup behaviors and encapsulating some of the error handling.
	/// </summary>
	public class SessionFactoryHelper
	{
		public static IQueryable FindQueryableUsingImports(ISessionFactoryImplementor sfi, string className)
		{
			string importedClassName = sfi.GetImportedClassName(className);
			
			if (importedClassName == null)
			{
				return null;
			}
			
			return (IQueryable) sfi.GetEntityPersister(importedClassName, false);
		}

		public static System.Type GetImportedClass(ISessionFactoryImplementor sfi, string className)
		{
			string importedName = sfi.GetImportedClassName(className);

			if (importedName == null)
			{
				return null;
			}

			return System.Type.GetType(importedName, false);
		}
	}
}