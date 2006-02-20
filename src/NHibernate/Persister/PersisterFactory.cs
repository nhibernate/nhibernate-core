using System;
using System.Reflection;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;

namespace NHibernate.Persister
{
	/// <summary>
	/// Factory for <c>IClassPersister</c> and <c>ICollectionPersister</c> instances.
	/// </summary>
	public sealed class PersisterFactory
	{
		//TODO: make ClassPersisters *not* depend on ISessionFactoryImplementor
		// interface, if possible

		private PersisterFactory()
		{
		}

		private static readonly System.Type[ ] PersisterConstructorArgs = new System.Type[ ] {typeof( PersistentClass ), typeof( ISessionFactoryImplementor )};

		// TODO: is it really neceassry to provide Configuration to CollectionPersisters ? Should it not be enough with associated class ?
		// or why does ClassPersister's not get access to configuration ?
		private static readonly System.Type[ ] CollectionPersisterConstructorArgs = new System.Type[] { typeof( Mapping.Collection ), typeof( ISessionFactoryImplementor ) };

		/// <summary>
		/// Creates a built in Entity Persister or a custom Persister.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static IClassPersister CreateClassPersister( PersistentClass model, ISessionFactoryImplementor factory )
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

		public static ICollectionPersister CreateCollectionPersister( Mapping.Collection model, ISessionFactoryImplementor factory )
		{
			System.Type persisterClass = model.CollectionPersisterClass;
			if ( persisterClass == null )
			{
				// default behaviour
				return model.IsOneToMany ? 
					(ICollectionPersister) new OneToManyPersister( model, factory ) :
					(ICollectionPersister) new BasicCollectionPersister( model, factory ) ;
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
			catch( TargetInvocationException tie )
			{
				Exception e = tie.InnerException;
				if( e is HibernateException )
				{
					throw e;
				}
				else
				{
					throw new MappingException( "Could not instantiate persister " + persisterClass.Name, e );
				}
			}
			catch( Exception e )
			{
				throw new MappingException( "Could not instantiate persister " + persisterClass.Name, e );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persisterClass"></param>
		/// <param name="model"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static ICollectionPersister Create( System.Type persisterClass, Mapping.Collection model, ISessionFactoryImplementor factory )
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
				return ( ICollectionPersister ) pc.Invoke( new object[ ] {model, factory} );
			}
			catch( TargetInvocationException tie )
			{
				Exception e = tie.InnerException;
				if( e is HibernateException )
				{
					throw e;
				}
				else
				{
					throw new MappingException( "Could not instantiate collection persister " + persisterClass.Name, e );
				}
			}
			catch( Exception e )
			{
				throw new MappingException( "Could not instantiate collection persister " + persisterClass.Name, e );
			}
		}
	}
}