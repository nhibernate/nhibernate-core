using System;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
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

		// TODO: is it really neceassry to provide Configuration to CollectionPersisters ? Should it not be enough with associated class ?
		// or why does ClassPersister's not get access to configuration ?

		private PersisterFactory()
		{
		}

		private static readonly System.Type[ ] PersisterConstructorArgs = new System.Type[ ] {typeof( PersistentClass ), typeof( ISessionFactoryImplementor )};

		private static readonly System.Type[ ] CollectionPersisterConstructorArgs = new System.Type[] { typeof( Mapping.Collection ), typeof( Configuration ), typeof( ISessionFactoryImplementor ) };


		/// <summary>
		/// Creates a built in Entity Persister or a custom Persister.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static IClassPersister Create( PersistentClass model, ISessionFactoryImplementor factory )
		{
			System.Type persisterClass = model.ClassPersisterClass;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cfg"></param>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static ICollectionPersister CreateCollectionPersister( Configuration cfg, Mapping.Collection model, ISessionFactoryImplementor factory )
		{
			/*
			return new CollectionPersister( model, cfg, factory );
			*/
			
			System.Type persisterClass = model.CollectionPersisterClass;
			if ( persisterClass == null )
			{
				// default behaviour
				return model.IsOneToMany ? 
					(ICollectionPersister) new OneToManyPersister( model, cfg, factory ) :
					(ICollectionPersister) new BasicCollectionPersister( model, cfg, factory ) ;
			}
			else
			{
				return Create( persisterClass, cfg, model, factory );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persisterClass"></param>
		/// <param name="cfg"></param>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static ICollectionPersister Create( System.Type persisterClass, Configuration cfg, Mapping.Collection model, ISessionFactoryImplementor factory )
		{
			ConstructorInfo pc;
			try
			{
				pc = persisterClass.GetConstructor( PersisterFactory.CollectionPersisterConstructorArgs );
			}
			catch( Exception e )
			{
				throw new MappingException( "Could not get constructor for " + persisterClass.Name, e );
			}

			try
			{
				return ( ICollectionPersister ) pc.Invoke( new object[ ] {model, cfg, factory} );
			}
				//TODO: add more specialized error catches
			catch( Exception e )
			{
				throw new MappingException( "Could not instantiate persister " + persisterClass.Name, e );
			}
		}
	}
}