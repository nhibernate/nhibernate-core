using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Mapping;

namespace NHibernate.Persister
{
	/// <summary>
	/// Factory for <c>ClassPersister</c> instances.
	/// </summary>
	public sealed class PersisterFactory
	{
		//TODO: make ClassPersisters *not* depend on ISessionFactoryImplementor
		// interface, if possible

		private PersisterFactory()
		{
		}

		private static readonly System.Type[ ] PersisterConstructorArgs = new System.Type[ ] {typeof( PersistentClass ), typeof( ISessionFactoryImplementor )};


		/// <summary>
		/// Creates a built in Entity Persister or a custom Persister.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static IClassPersister Create( PersistentClass model, ISessionFactoryImplementor factory )
		{
			System.Type persisterClass = model.Persister;

			if( persisterClass == null || persisterClass == typeof( EntityPersister ) )
			{
				return new EntityPersister( model, factory );
			}
			else if( persisterClass == typeof( NormalizedEntityPersister ) )
			{
				return new NormalizedEntityPersister( model, factory );
			}
			else
			{
				return Create( persisterClass, model, factory );
			}
		}

		/// <summary>
		/// Creates a specific Persister - could be a built in or custom persister.
		/// </summary>
		/// <param name="persisterClass"></param>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static IClassPersister Create( System.Type persisterClass, PersistentClass model, ISessionFactoryImplementor factory )
		{
			ConstructorInfo pc;
			try
			{
				pc = persisterClass.GetConstructor( PersisterFactory.PersisterConstructorArgs );
			}
			catch( Exception e )
			{
				throw new MappingException( "Could not get constructor for " + persisterClass.Name, e );
			}

			try
			{
				return ( IClassPersister ) pc.Invoke( new object[ ] {model, factory} );
			}
				//TODO: add more specialized error catches
			catch( Exception e )
			{
				throw new MappingException( "Could not instantiate persister " + persisterClass.Name, e );
			}

		}

	}
}