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
		public static IQueryable FindQueryableUsingImports( ISessionFactoryImplementor sfi, string className )
		{
			string importedClassName = sfi.GetImportedClassName( className );
			if ( importedClassName == null ) 
			{
				return null;
			}

			try 
			{
				return ( IQueryable ) sfi.GetEntityPersister( importedClassName );
			}
			catch ( MappingException )
			{
				return null;
			}
		}
	}
}
