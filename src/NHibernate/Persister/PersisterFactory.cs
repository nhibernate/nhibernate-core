using System;
using System.Reflection;
using System.Text;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Persister
{
	/// <summary>
	/// Factory for <c>IEntityPersister</c> and <c>ICollectionPersister</c> instances.
	/// </summary>
	public sealed class PersisterFactory
	{
		//TODO: make ClassPersisters *not* depend on ISessionFactoryImplementor
		// interface, if possible

		private PersisterFactory()
		{
		}

		private static readonly System.Type[] PersisterConstructorArgs = new System.Type[]
			{
				typeof(PersistentClass),
				typeof(ICacheConcurrencyStrategy),
				typeof(ISessionFactoryImplementor),
				typeof(IMapping)
			};

		// TODO: is it really necessary to provide Configuration to CollectionPersisters ? Should it not be enough with associated class ?
		// or why does ClassPersister's not get access to configuration ?
		private static readonly System.Type[] CollectionPersisterConstructorArgs = new System.Type[]
			{
				typeof(Mapping.Collection),
				typeof(ICacheConcurrencyStrategy),
				typeof(ISessionFactoryImplementor)
			};

		private static readonly System.Type[] CollectionPersisterConstructor2Args = new System.Type[]
			{
				typeof(Mapping.Collection),
				typeof(ICacheConcurrencyStrategy),
				typeof(Configuration),
				typeof(ISessionFactoryImplementor)
			};

		/// <summary>
		/// Creates a built in Entity Persister or a custom Persister.
		/// </summary>
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

		public static ICollectionPersister CreateCollectionPersister(Configuration cfg, Mapping.Collection model, ICacheConcurrencyStrategy cache,
																	 ISessionFactoryImplementor factory)
		{
			System.Type persisterClass = model.CollectionPersisterClass;
			if (persisterClass == null)
			{
				// default behaviour
				return
					model.IsOneToMany
						? (ICollectionPersister) new OneToManyPersister(model, cache, cfg, factory)
						: (ICollectionPersister) new BasicCollectionPersister(model, cache, cfg, factory);
			}
			else
			{
				return Create(persisterClass, model, cache, factory, cfg);
			}
		}

		/// <summary>
		/// Creates a specific Persister - could be a built in or custom persister.
		/// </summary>
		public static IEntityPersister Create(System.Type persisterClass, PersistentClass model,
											  ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory,
											  IMapping cfg)
		{
			ConstructorInfo pc;
			try
			{
				pc = persisterClass.GetConstructor(PersisterConstructorArgs);
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
					throw e;
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

		public static ICollectionPersister Create(System.Type persisterClass, Mapping.Collection model,
												  ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory, Configuration cfg)
		{
			ConstructorInfo pc;
			var use4Parameters = false;
			try
			{
				pc = persisterClass.GetConstructor(CollectionPersisterConstructorArgs);
				if (pc == null)
				{
					use4Parameters = true;
					pc = persisterClass.GetConstructor(CollectionPersisterConstructor2Args);
				}
			}
			catch (Exception e)
			{
				throw new MappingException("Could not get constructor for " + persisterClass.Name, e);
			}
			if(pc == null)
			{
				var messageBuilder = new StringBuilder();
				messageBuilder.AppendLine("Could not find a public constructor for " + persisterClass.Name +";");
				messageBuilder.AppendLine("- The ctor may have " + CollectionPersisterConstructorArgs.Length + " parameters of types (in order):");
				System.Array.ForEach(CollectionPersisterConstructorArgs, t=> messageBuilder.AppendLine(t.FullName));
				messageBuilder.AppendLine();
				messageBuilder.AppendLine("- The ctor may have " + CollectionPersisterConstructor2Args.Length + " parameters of types (in order):");
				System.Array.ForEach(CollectionPersisterConstructor2Args, t => messageBuilder.AppendLine(t.FullName));
				throw new MappingException(messageBuilder.ToString());
			}
			try
			{
				if (!use4Parameters)
				{
					return (ICollectionPersister) pc.Invoke(new object[] {model, cache, factory});
				}
				return (ICollectionPersister)pc.Invoke(new object[] { model, cache, cfg, factory });
			}
			catch (TargetInvocationException tie)
			{
				Exception e = tie.InnerException;
				if (e is HibernateException)
				{
					throw e;
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
	}
}
