using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Hql.Util
{
	/// <summary>
	/// Wraps SessionFactoryImpl, adding more lookup behaviors and encapsulating some of the error handling.
	/// </summary>
	public class SessionFactoryHelper
	{
		public static IQueryable FindQueryableUsingImports(ISessionFactoryImplementor sfi, string className)
		{
			return FindEntityPersisterUsingImports(sfi, className) as IQueryable;
		}

		public static IEntityPersister FindEntityPersisterUsingImports(ISessionFactoryImplementor sfi, string className)
		{
			// NH : short cut
			if (string.IsNullOrEmpty(className))
			{
				return null;
			}

			if (!char.IsLetter(className[0]) && !className[0].Equals('_'))
			{
				return null;
			}

			// NH : this method prevent unrecognized class when entityName != class.FullName
			// this is a patch for the TODO below
			var possibleResult = sfi.TryGetEntityPersister(GetEntityName(className));
			if (possibleResult != null)
			{
				return possibleResult;
			}

			string importedClassName = sfi.GetImportedClassName(className);

			if (importedClassName == null)
			{
				return null;
			}
			// NH: This method don't work if entityName != class.FullName
			return sfi.TryGetEntityPersister(GetEntityName(importedClassName));
		}

		private static string GetEntityName(string assemblyQualifiedName)
		{
			/* *********************************************************************************************************
			 * TODO NH Different impl.: we need to resolve the matter between FullName-AssemblyQualifiedName-EntityName-Name
			 * GetImportedClassName in h3.2.5 return the entityName that, in many cases but not all, should be the
			 * MappesClass.FullName. The value returned by GetImportedClassName, in this case, is used to find the persister.
			 * A possible solution would be to use the same behavior of H3.2.5 but we start to have some problems (performance)
			 * when we try to use the result of GetImportedClassName to create an instance and we completely lost a way
			 * to link an entityName with its AssemblyQualifiedName (strongly typed).
			 * I would like to maitain <imports> like the holder of the association of an entityName (or a class Name) and
			 * its Type (in the future: Dictionary<string, System.Type> imports;)
			 * *********************************************************************************************************
			 */
			return TypeNameParser.Parse(assemblyQualifiedName).Type;
		}

		public static System.Type GetImportedClass(ISessionFactoryImplementor sfi, string className)
		{
			string importedName = sfi.GetImportedClassName(className);

			if (importedName == null)
			{
				return null;
			}

			// NH Different implementation: our sessionFactory.Imports hold AssemblyQualifiedName
			return System.Type.GetType(importedName, false);
		}
	}
}