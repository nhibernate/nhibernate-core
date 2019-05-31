using System;
using System.Reflection;
using System.Text;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Util;

namespace NHibernate.Persister
{
	/// <summary>
	/// Factory for <c>IEntityPersister</c> and <c>ICollectionPersister</c> instances.
	/// </summary>
	public static class PersisterFactory
	{
		//TODO: make ClassPersisters *not* depend on ISessionFactoryImplementor
		// interface, if possible

		/// <summary>
		/// Creates a built in Entity Persister or a custom Persister.
		/// </summary>
		[Obsolete("Use overload with cacheByTenant delegate")]
		public static IEntityPersister CreateClassPersister(PersistentClass model, ICacheConcurrencyStrategy cache,
															ISessionFactoryImplementor factory, IMapping cfg)
		{
			System.Type persisterClass = model.EntityPersisterClass;

			if (persisterClass == null || persisterClass == typeof(SingleTableEntityPersister))
			{
				return new SingleTableEntityPersister(model, cache, factory, cfg);
			}
			else if (persisterClass == typeof(JoinedSubclassEntityPersister))
			{
				return new JoinedSubclassEntityPersister(model, cache, factory, cfg);
			}
			else if (persisterClass == typeof(UnionSubclassEntityPersister))
			{
				return new UnionSubclassEntityPersister(model, cache, factory, cfg);
			}
			else
			{
				return Create(persisterClass, model, cache, factory, cfg);
			}
		}

		public static IEntityPersister CreateClassPersister(PersistentClass model, Func<string, ICacheConcurrencyStrategy> cacheByTenant, SessionFactoryImpl factory, IMapping cfg)
		{
			System.Type persisterClass = model.EntityPersisterClass;

			if (persisterClass == null || persisterClass == typeof(SingleTableEntityPersister))
			{
				return new SingleTableEntityPersister(model, cacheByTenant, factory, cfg);
			}
			else if (persisterClass == typeof(JoinedSubclassEntityPersister))
			{
				return new JoinedSubclassEntityPersister(model, cacheByTenant, factory, cfg);
			}
			else if (persisterClass == typeof(UnionSubclassEntityPersister))
			{
				return new UnionSubclassEntityPersister(model, cacheByTenant, factory, cfg);
			}
			else
			{
				return Create(persisterClass, model, cacheByTenant, factory, cfg);
			}
		}

		//Since 5.3
		[Obsolete("Use overload with cacheByTenant delegate")]
		public static ICollectionPersister CreateCollectionPersister(Mapping.Collection model, ICacheConcurrencyStrategy cache,
																	 ISessionFactoryImplementor factory)
		{
			System.Type persisterClass = model.CollectionPersisterClass;
			if (persisterClass == null)
			{
				// default behaviour
				return
					model.IsOneToMany
						? (ICollectionPersister) new OneToManyPersister(model, cache, factory)
						: (ICollectionPersister) new BasicCollectionPersister(model, cache, factory);
			}
			else
			{
				return Create(persisterClass, model, cache, factory);
			}
		}

		public static ICollectionPersister CreateCollectionPersister(Mapping.Collection model, Func<string, ICacheConcurrencyStrategy> cacheByTenant, SessionFactoryImpl factory)
		{
			System.Type persisterClass = model.CollectionPersisterClass;
			if (persisterClass == null)
			{
				// default behaviour
				if (model.IsOneToMany)
					return new OneToManyPersister(model, cacheByTenant, factory);
				return new BasicCollectionPersister(model, cacheByTenant, factory);
			}

			return Create(persisterClass, model, cacheByTenant, factory);
		}

		/// <summary>
		/// Creates a specific Persister - could be a built in or custom persister.
		/// </summary>
		public static IEntityPersister Create(System.Type persisterClass, PersistentClass model,
											Func<string, ICacheConcurrencyStrategy> cache, ISessionFactoryImplementor factory,
											IMapping cfg)
		{
			ConstructorInfo pc;
			try
			{
				pc = persisterClass.GetConstructor(new[] {
					TypeOf(model),
					TypeOf(cache),
					TypeOf(factory),
					TypeOf(cfg)
				});

#pragma warning disable CS0618 // Type or member is obsolete
				//To support legacy custom persisters
				if (pc == null)
				{
					return Create(persisterClass, model, cache(null), factory, cfg);
				}
#pragma warning restore CS0618 // Type or member is obsolete
			}
			catch (Exception e)
			{
				throw new MappingException("Could not get constructor for " + persisterClass.Name, e);
			}

			try
			{
				return (IEntityPersister) pc.Invoke(new object[] {model, cache, factory, cfg});
			}
			catch (TargetInvocationException tie)
			{
				Exception e = tie.InnerException;
				if (e is HibernateException)
				{
					throw ReflectHelper.UnwrapTargetInvocationException(tie);
				}
				else
				{
					throw new MappingException("Could not instantiate persister " + persisterClass.Name, e);
				}
			}
			catch (Exception e)
			{
				throw new MappingException("Could not instantiate persister " + persisterClass.Name, e);
			}
		}

		[Obsolete("Please use overload with cacheByTenant delegate")]
		public static IEntityPersister Create(System.Type persisterClass, PersistentClass model,
											  ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory,
											  IMapping cfg)
		{
			ConstructorInfo pc;
			try
			{
				pc = persisterClass.GetConstructor(new[] {
					TypeOf(model),
					TypeOf(cache),
					TypeOf(factory),
					TypeOf(cfg)
				});
			}
			catch (Exception e)
			{
				throw new MappingException("Could not get constructor for " + persisterClass.Name, e);
			}

			try
			{
				return (IEntityPersister) pc.Invoke(new object[] {model, cache, factory, cfg});
			}
			catch (TargetInvocationException tie)
			{
				Exception e = tie.InnerException;
				if (e is HibernateException)
				{
					throw ReflectHelper.UnwrapTargetInvocationException(tie);
				}
				else
				{
					throw new MappingException("Could not instantiate persister " + persisterClass.Name, e);
				}
			}
			catch (Exception e)
			{
				throw new MappingException("Could not instantiate persister " + persisterClass.Name, e);
			}
		}

		//Since 5.3
		[Obsolete("Use overload with cacheByTenant delegate")]
		public static ICollectionPersister Create(System.Type persisterClass, Mapping.Collection model,
												  ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory)
		{
			ConstructorInfo pc;
			var ctorArgs = new[] {TypeOf(model), TypeOf(cache), TypeOf(factory)};
			try
			{
				pc = persisterClass.GetConstructor(ctorArgs);
			}
			catch (Exception e)
			{
				throw new MappingException("Could not get constructor for " + persisterClass.Name, e);				
			}
			if(pc == null)
			{
				var messageBuilder = new StringBuilder();
				messageBuilder.Append("Could not find a public constructor for ").Append(persisterClass.Name).AppendLine(";");
				messageBuilder.Append("- The ctor may have ").Append(ctorArgs.Length).AppendLine(" parameters of types (in order):");
				System.Array.ForEach(ctorArgs, t=> messageBuilder.AppendLine(t.FullName));
				throw new MappingException(messageBuilder.ToString());
			}
			try
			{
				return (ICollectionPersister) pc.Invoke(new object[] {model, cache, factory});
			}
			catch (TargetInvocationException tie)
			{
				Exception e = tie.InnerException;
				if (e is HibernateException)
				{
					throw ReflectHelper.UnwrapTargetInvocationException(tie);
				}
				else
				{
					throw new MappingException("Could not instantiate collection persister " + persisterClass.Name, e);
				}
			}
			catch (Exception e)
			{
				throw new MappingException("Could not instantiate collection persister " + persisterClass.Name, e);
			}
		}

		public static ICollectionPersister Create(System.Type persisterClass, Mapping.Collection model,
												  Func<string, ICacheConcurrencyStrategy> cacheByTenant, ISessionFactoryImplementor factory)
		{
			ConstructorInfo pc;
			var ctorArgs = new[]
			{
				TypeOf(model),
				TypeOf(cacheByTenant),
				TypeOf(factory)
			};
			try
			{
				pc = persisterClass.GetConstructor(ctorArgs);

#pragma warning disable CS0618 // Type or member is obsolete
				//To support legacy custom persisters
				if (pc == null)
				{
					return Create(persisterClass, model, cacheByTenant(null), factory);
				}
#pragma warning restore CS0618 // Type or member is obsolete
			}
			catch (Exception e)
			{
				throw new MappingException("Could not get constructor for " + persisterClass.Name, e);
			}
			if(pc == null)
			{
				var messageBuilder = new StringBuilder();
				messageBuilder.Append("Could not find a public constructor for ").Append(persisterClass.Name).AppendLine(";");
				messageBuilder.Append("- The ctor may have ").Append(ctorArgs.Length).AppendLine(" parameters of types (in order):");
				System.Array.ForEach(ctorArgs, t=> messageBuilder.AppendLine(t.FullName));
				throw new MappingException(messageBuilder.ToString());
			}
			try
			{
				return (ICollectionPersister) pc.Invoke(new object[] {model, cacheByTenant, factory});
			}
			catch (TargetInvocationException tie)
			{
				Exception e = tie.InnerException;
				if (e is HibernateException)
				{
					throw ReflectHelper.UnwrapTargetInvocationException(tie);
				}
				else
				{
					throw new MappingException("Could not instantiate collection persister " + persisterClass.Name, e);
				}
			}
			catch (Exception e)
			{
				throw new MappingException("Could not instantiate collection persister " + persisterClass.Name, e);
			}
		}

		private static System.Type TypeOf<T>(T param)
		{
			return typeof(T);
		}
	}
}
