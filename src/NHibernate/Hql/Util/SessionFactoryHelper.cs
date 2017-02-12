using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Hql.Util
{
	/// <summary>
	/// Wraps SessionFactoryImpl, adding more lookup behaviors and encapsulating some of the error handling.
	/// </summary>
	public class SessionFactoryHelper
	{
		private readonly ISessionFactoryImplementor sfi;
		private readonly IDictionary<string,CollectionPropertyMapping> collectionPropertyMappingByRole = 
			new Dictionary<string,CollectionPropertyMapping>();

		public SessionFactoryHelper(ISessionFactoryImplementor sfi)
		{
			this.sfi = sfi;
		}

		public IQueryable FindQueryableUsingImports(string className)
		{
			return FindEntityPersisterUsingImports(className) as IQueryable;
		}

		public IEntityPersister FindEntityPersisterUsingImports(string className)
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



		/// <summary>
		/// Locate the collection persister by the collection role.
		/// </summary>
		/// <param name="role">The collection role name.</param>
		/// <returns>The defined CollectionPersister for this collection role, or null.</returns>
		public IQueryableCollection GetCollectionPersister(String role)
		{
			try
			{
				return (IQueryableCollection)sfi.GetCollectionPersister(role);
			}
			catch (InvalidCastException)
			{
				throw new QueryException("collection is not queryable: " + role);
			}
			catch (Exception)
			{
				throw new QueryException("collection not found: " + role);
			}
		}

		/// <summary>
		/// Locate the persister by class or entity name, requiring that such a persister
		/// exists
		/// </summary>
		/// <param name="name">The class or entity name</param>
		/// <returns>The defined persister for this entity</returns>
		public IEntityPersister RequireClassPersister(String name)
		{
			IEntityPersister cp = FindEntityPersisterByName(name);
			if (cp == null)
			{
				throw new QuerySyntaxException(name + " is not mapped");
			}

			return cp;
		}

		/// <summary>
		/// Locate the persister by class or entity name.
		/// </summary>
		/// <param name="name">The class or entity name</param>
		/// <returns>The defined persister for this entity, or null if none found.</returns>
		private IEntityPersister FindEntityPersisterByName(String name)
		{
			// First, try to get the persister using the given name directly.
			try
			{
				return sfi.GetEntityPersister(name);
			}
			catch (MappingException)
			{
				// unable to locate it using this name
			}

			// If that didn't work, try using the 'import' name.
			String importedClassName = sfi.GetImportedClassName(name);
			if (importedClassName == null)
			{
				return null;
			}
			return sfi.GetEntityPersister(importedClassName);
		}


		public System.Type GetImportedClass(string className)
		{
			string importedName = sfi.GetImportedClassName(className);

			if (importedName == null)
			{
				return null;
			}

			// NH Different implementation: our sessionFactory.Imports hold AssemblyQualifiedName
			return System.Type.GetType(importedName, false);
		}


		/// <summary>
		/// Retrieve a PropertyMapping describing the given collection role.
		/// </summary>
		/// <param name="role">The collection role for which to retrieve the property mapping.</param>
		/// <returns>The property mapping.</returns>
		public IPropertyMapping GetCollectionPropertyMapping(String role)
		{
			return collectionPropertyMappingByRole[role];
		}


		/* Locate the collection persister by the collection role, requiring that
		* such a persister exist.
		*
		* @param role The collection role name.
		* @return The defined CollectionPersister for this collection role.
		* @throws QueryException Indicates that the collection persister could not be found.
		*/
		public IQueryableCollection RequireQueryableCollection(String role)
		{
			try
			{
				IQueryableCollection queryableCollection = (IQueryableCollection)sfi
						.GetCollectionPersister(role);
				if (queryableCollection != null)
				{
					collectionPropertyMappingByRole.Add(role,
							new CollectionPropertyMapping(queryableCollection));
				}
				return queryableCollection;
			}
			catch (InvalidCastException)
			{
				throw new QueryException(
						"collection role is not queryable: " + role);
			}
			catch (Exception)
			{
				throw new QueryException("collection role not found: "
						+ role);
			}
		}
	}
}