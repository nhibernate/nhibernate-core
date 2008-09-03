using System;
using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Provides the base functionality to Handle Member calls into a dynamically
	/// generated NHibernate Proxy.
	/// </summary>
	/// <remarks>
	/// This could be an extension point later if the .net framework ever gets a Proxy
	/// class that is similar to the java.lang.reflect.Proxy or if a library similar
	/// to cglib was made in .net.
	/// </remarks>
	[Serializable]
	public abstract class AbstractLazyInitializer : ILazyInitializer
	{
		/// <summary>
		/// If this is returned by Invoke then the subclass needs to Invoke the
		/// method call against the object that is being proxied.
		/// </summary>
		protected static readonly object InvokeImplementation = new object();

		private object _target = null;
		private bool initialized;
		private object _id;
		[NonSerialized]
		private ISessionImplementor _session;
		private bool unwrap;
		private readonly string _entityName;

		/// <summary>
		/// Create a LazyInitializer to handle all of the Methods/Properties that are called
		/// on the Proxy.
		/// </summary>
		/// <param name="entityName">The entityName</param>
		/// <param name="id">The Id of the Object we are Proxying.</param>
		/// <param name="session">The ISession this Proxy is in.</param>
		protected internal AbstractLazyInitializer(string entityName, object id, ISessionImplementor session)
		{
			_id = id;
			_session = session;
			_entityName = entityName;
		}

		/// <summary>
		/// Perform an ImmediateLoad of the actual object for the Proxy.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Proxy has no Session or the Session is closed or disconnected.
		/// </exception>
		public void Initialize()
		{
			if (!initialized)
			{
				if (_session == null)
				{
					throw new LazyInitializationException("Could not initialize proxy - no Session.");
				}
				else if (!_session.IsOpen)
				{
					throw new LazyInitializationException("Could not initialize proxy - the owning Session was closed.");
				}
				else if (!_session.IsConnected)
				{
					throw new LazyInitializationException("Could not initialize proxy - the owning Session is disconnected.");
				}
				else
				{
					_target = _session.ImmediateLoad(_entityName, _id);
					initialized = true;
					CheckTargetState();
				}
			}
			else
			{
				CheckTargetState();
			}
		}

		private void CheckTargetState()
		{
			if (!unwrap)
			{
				if (_target == null)
				{
					Session.Factory.EntityNotFoundDelegate.HandleEntityNotFound(_entityName, _id);
				}
			}
		}

		/// <summary></summary>
		public object Identifier
		{
			get { return _id; }
			set { _id = value; }
		}

		public abstract System.Type PersistentClass { get;}

		public bool IsUninitialized
		{
			get { return !initialized; }
		}

		public bool Unwrap
		{
			get { return unwrap; }
			set { unwrap = value; }
		}

		/// <summary></summary>
		public ISessionImplementor Session
		{
			get { return _session; }
			set
			{
				if (value != _session)
				{
					if (value != null && IsConnectedToSession)
					{
						//TODO: perhaps this should be some other RuntimeException...
						throw new LazyInitializationException("Illegally attempted to associate a proxy with two open Sessions");
					}
					else
					{
						_session = value;
					}
				}
			}
		}

		public string EntityName
		{
			get { return _entityName; }
		}

		protected internal bool IsConnectedToSession
		{
			get{return _session != null && _session.IsOpen && _session.PersistenceContext.ContainsProxy((this as INHibernateProxy));}
		}

		protected internal object Target
		{
			get { return _target; }
		}

		/// <summary>
		/// Return the Underlying Persistent Object, initializing if necessary.
		/// </summary>
		/// <returns>The Persistent Object this proxy is Proxying.</returns>
		public object GetImplementation()
		{
			Initialize();
			return _target;
		}

		/// <summary>
		/// Return the Underlying Persistent Object in a given <see cref="ISession"/>, or null.
		/// </summary>
		/// <param name="s">The Session to get the object from.</param>
		/// <returns>The Persistent Object this proxy is Proxying, or <see langword="null" />.</returns>
		public object GetImplementation(ISessionImplementor s)
		{
			EntityKey key = new EntityKey(Identifier, s.Factory.GetEntityPersister(EntityName), s.EntityMode);
			return s.PersistenceContext.GetEntity(key);
		}

		public void SetImplementation(object target)
		{
			_target = target;
			initialized = true;
		}
	}
}
