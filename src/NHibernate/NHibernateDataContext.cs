using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Linq;

namespace NHibernate
{
	public abstract class NHibernateDataContext : MarshalByRefObject, IUpdatable, IDisposable
	{
		private readonly ISet<Object> entitiesToUpdate = new HashSet<Object>();
		private readonly ISet<Object> entitiesToRemove = new HashSet<Object>();

		protected NHibernateDataContext(ISessionFactory sessionFactory)
		{
			this.Session = sessionFactory.OpenSession();
		}

		~NHibernateDataContext()
		{
			this.Dispose(false);
		}

		protected ISession Session
		{
			get;
			private set;
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				this.Session.Dispose();
				this.Session = null;

				this.entitiesToRemove.Clear();
				this.entitiesToUpdate.Clear();
			}
		}

		public IQueryable<T> Query<T>() where T : class
		{
			return (this.Session.Query<T>());
		}

		public T Get<T>(Object id) where T : class
		{
			return (this.Session.Get<T>(id));
		}

		public Status Status<T>(T entity) where T : class
		{			
			return (this.Session.GetSessionImplementation().PersistenceContext.GetEntry(entity).Status);
		}

		public void Save<T>(T entity) where T : class
		{
			this.Session.Save(entity);
		}

		public Boolean Contains<T>(T entity) where T : class
		{
			return (this.Session.Contains(entity));
		}

		public T Attach<T>(T entity) where T: class
		{
			return(this.Session.Merge(entity));
		}

		public void Detach<T>(T entity) where T : class
		{
			this.Session.Evict(entity);
		}

		public void Refresh<T>(T entity) where T : class
		{
			this.Session.Refresh(entity);
		}

		public void Delete<T>(T entity) where T : class
		{
			this.Session.Delete(entity);
		}

		#region IUpdatable members
		/// <summary>
		/// Creates the resource of the given type and belonging to the given container
		/// </summary>
		/// <param name="containerName">container name to which the resource needs to be added</param>
		/// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
		/// <returns>Object representing a resource of given type and belonging to the given container</returns>
		Object IUpdatable.CreateResource(String containerName, String fullTypeName)
		{
			System.Type t = System.Type.GetType(fullTypeName, true);
			Object resource = Activator.CreateInstance(t);

			this.entitiesToUpdate.Add(resource);

			return resource;
		}

		/// <summary>
		/// Gets the resource of the given type that the query points to
		/// </summary>
		/// <param name="query">query pointing to a particular resource</param>
		/// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
		/// <returns>Object representing a resource of given type and as referenced by the query</returns>
		Object IUpdatable.GetResource(IQueryable query, String fullTypeName)
		{
			Object resource = null;

			foreach (Object item in query)
			{
				if (resource != null)
				{
					throw (new DataServiceException("The query must return a single resource"));
				}

				resource = item;
			}

			if (resource == null)
			{
				throw (new DataServiceException(404, "Resource not found"));
			}

			// fullTypeName can be null for deletes
			if ((fullTypeName != null) && (resource.GetType().FullName != fullTypeName))
			{
				throw (new Exception("Unexpected type for resource"));
			}

			return (resource);
		}


		/// <summary>
		/// Resets the value of the given resource to its default value
		/// </summary>
		/// <param name="resource">resource whose value needs to be reset</param>
		/// <returns>same resource with its value reset</returns>
		Object IUpdatable.ResetResource(Object resource)
		{
			return (resource);
		}

		/// <summary>
		/// Sets the value of the given property on the target Object
		/// </summary>
		/// <param name="targetResource">target Object which defines the property</param>
		/// <param name="propertyName">name of the property whose value needs to be updated</param>
		/// <param name="propertyValue">value of the property</param>
		void IUpdatable.SetValue(Object targetResource, String propertyName, Object propertyValue)
		{
			PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName);
			propertyInfo.SetValue(targetResource, propertyValue, null);

			if (this.entitiesToUpdate.Contains(targetResource) == false)
			{
				this.entitiesToUpdate.Add(targetResource);
			}
		}

		/// <summary>
		/// Gets the value of the given property on the target Object
		/// </summary>
		/// <param name="targetResource">target Object which defines the property</param>
		/// <param name="propertyName">name of the property whose value needs to be updated</param>
		/// <returns>the value of the property for the given target resource</returns>
		Object IUpdatable.GetValue(Object targetResource, String propertyName)
		{
			PropertyInfo propertyInfo = targetResource.GetType().GetProperty(propertyName);
			return (propertyInfo.GetValue(targetResource, null));
		}

		/// <summary>
		/// Sets the value of the given reference property on the target Object
		/// </summary>
		/// <param name="targetResource">target Object which defines the property</param>
		/// <param name="propertyName">name of the property whose value needs to be updated</param>
		/// <param name="propertyValue">value of the property</param>
		void IUpdatable.SetReference(Object targetResource, String propertyName, Object propertyValue)
		{
			(this as IUpdatable).SetValue(targetResource, propertyName, propertyValue);
		}

		/// <summary>
		/// Adds the given value to the collection
		/// </summary>
		/// <param name="targetResource">target Object which defines the property</param>
		/// <param name="propertyName">name of the property whose value needs to be updated</param>
		/// <param name="resourceToBeAdded">value of the property which needs to be added</param>
		void IUpdatable.AddReferenceToCollection(Object targetResource, String propertyName, Object resourceToBeAdded)
		{
			PropertyInfo pi = targetResource.GetType().GetProperty(propertyName);

			if (pi == null)
			{
				throw (new Exception("Can't find property"));
			}

			IList collection = pi.GetValue(targetResource, null) as IList;

			if (collection != null)
			{
				collection.Add(resourceToBeAdded);
			}

			if (this.entitiesToUpdate.Contains(targetResource) == false)
			{
				this.entitiesToUpdate.Add(targetResource);
			}
		}

		/// <summary>
		/// Removes the given value from the collection
		/// </summary>
		/// <param name="targetResource">target Object which defines the property</param>
		/// <param name="propertyName">name of the property whose value needs to be updated</param>
		/// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
		void IUpdatable.RemoveReferenceFromCollection(Object targetResource, String propertyName, Object resourceToBeRemoved)
		{
			PropertyInfo pi = targetResource.GetType().GetProperty(propertyName);
			
			if (pi == null)
			{
				throw (new Exception("Can't find property"));
			}
			
			IList collection = pi.GetValue(targetResource, null) as IList;

			if (collection != null)
			{
				collection.Remove(resourceToBeRemoved);
			}

			if (this.entitiesToUpdate.Contains(targetResource) == false)
			{
				this.entitiesToUpdate.Add(targetResource);
			}
		}

		/// <summary>
		/// Delete the given resource
		/// </summary>
		/// <param name="targetResource">resource that needs to be deleted</param>
		void IUpdatable.DeleteResource(Object targetResource)
		{
			this.entitiesToRemove.Add(targetResource);
		}

		/// <summary>
		/// Saves all the pending changes made till now
		/// </summary>
		void IUpdatable.SaveChanges()
		{
			FlushMode originalFlushMode = this.Session.FlushMode;

			using (ITransaction transaction = this.Session.BeginTransaction())
			{
				this.Session.FlushMode = FlushMode.Commit;

				foreach (Object entity in this.entitiesToUpdate)
				{
					this.Session.SaveOrUpdate(entity);
				}

				foreach (Object entity in this.entitiesToRemove)
				{
					this.Session.Delete(entity);
				}

				transaction.Commit();
			}

			this.Session.FlushMode = originalFlushMode;
		}

		/// <summary>
		/// Returns the actual instance of the resource represented by the given resource Object
		/// </summary>
		/// <param name="resource">Object representing the resource whose instance needs to be fetched</param>
		/// <returns>The actual instance of the resource represented by the given resource Object</returns>
		Object IUpdatable.ResolveResource(Object resource)
		{
			return (resource);
		}

		/// <summary>
		/// Revert all the pending changes.
		/// </summary>
		void IUpdatable.ClearChanges()
		{
			ISessionImplementor impl = this.Session.GetSessionImplementation();
			IPersistenceContext ctx = impl.PersistenceContext;

			foreach (Object obj in ctx.EntityEntries.Keys.OfType<Object>().ToArray())
			{
				this.Session.Evict(obj);

				if (this.entitiesToUpdate.Contains(obj) == true)
				{
					this.entitiesToUpdate.Remove(obj);
				}

				if (this.entitiesToRemove.Contains(obj) == true)
				{
					this.entitiesToRemove.Remove(obj);
				}
			}
		}
		#endregion

		#region IDisposable members
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}