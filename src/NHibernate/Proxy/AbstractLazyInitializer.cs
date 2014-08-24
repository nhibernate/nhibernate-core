using System;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

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
		private bool readOnly;
		private bool? readOnlyBeforeAttachedToSession;

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
			_entityName = entityName;
			
			if (session == null)
				UnsetSession();
			else
				SetSession(session);
		}

		/// <inheritdoc />
		public void SetSession(ISessionImplementor s)
		{
			if (s != _session)
			{
				// check for s == null first, since it is least expensive
				if (s == null)
				{
					UnsetSession();
				}
				else if (IsConnectedToSession)
				{
					//TODO: perhaps this should be some other RuntimeException...
					throw new HibernateException("illegally attempted to associate a proxy with two open Sessions");
				}
				else
				{
					// s != null
					_session = s;
					if (readOnlyBeforeAttachedToSession == null)
					{
						// use the default read-only/modifiable setting
						IEntityPersister persister = s.Factory.GetEntityPersister(_entityName);
						SetReadOnly(s.PersistenceContext.DefaultReadOnly || !persister.IsMutable);
					}
					else
					{
						// use the read-only/modifiable setting indicated during deserialization
						SetReadOnly(readOnlyBeforeAttachedToSession.Value);
						readOnlyBeforeAttachedToSession = null;
					}
				}
			}
		}
		
		/// <inheritdoc />
		public void UnsetSession()
		{
			_session = null;
			readOnly = false;
			readOnlyBeforeAttachedToSession = null;
		}
		
		protected internal bool IsConnectedToSession
		{
			get { return GetProxyOrNull() != null; }
		}
		
		/// <summary>
		/// Perform an ImmediateLoad of the actual object for the Proxy.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Proxy has no Session or the Session is closed or disconnected.
		/// </exception>
		public virtual void Initialize()
		{
			if (!initialized)
			{
				if (_session == null)
				{
					throw new LazyInitializationException(_entityName, _id, "Could not initialize proxy - no Session.");
				}
				else if (!_session.IsOpen)
				{
					throw new LazyInitializationException(_entityName, _id, "Could not initialize proxy - the owning Session was closed.");
				}
				else if (!_session.IsConnected)
				{
					throw new LazyInitializationException(_entityName, _id, "Could not initialize proxy - the owning Session is disconnected.");
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
						throw new LazyInitializationException(_entityName, _id, "Illegally attempted to associate a proxy with two open Sessions");
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
			EntityKey key = s.GenerateEntityKey(Identifier, s.Factory.GetEntityPersister(EntityName));
			return s.PersistenceContext.GetEntity(key);
		}

		public void SetImplementation(object target)
		{
			_target = target;
			initialized = true;
		}
		
		/// <inheritdoc />
		public bool IsReadOnlySettingAvailable
		{
			get { return (_session != null && !_session.IsClosed); }
		}
		
		/// <inheritdoc />
		public bool ReadOnly
		{
			get
			{
				ErrorIfReadOnlySettingNotAvailable();
				return readOnly;
			}
			set
			{
				ErrorIfReadOnlySettingNotAvailable();
			
				// only update if setting is different from current setting
				if (this.readOnly != value)
				{
					this.SetReadOnly(value);
				}
			}
		}

		private void ErrorIfReadOnlySettingNotAvailable()
		{
			if (_session == null)
				throw new TransientObjectException("Proxy is detached (i.e, session is null). The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");

			if (_session.IsClosed)
				throw new SessionException("Session is closed. The read-only/modifiable setting is only accessible when the proxy is associated with an open session.");
		}
		
		private static EntityKey GenerateEntityKeyOrNull(object id, ISessionImplementor s, string entityName)
		{
			if (id == null || s == null || entityName == null)
				return null;

			return s.GenerateEntityKey(id, s.Factory.GetEntityPersister(entityName));
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
		
		private object GetProxyOrNull()
		{
			EntityKey entityKey = GenerateEntityKeyOrNull(_id, _session, _entityName);
			if (entityKey != null && _session != null && _session.IsOpen)
			{
				return _session.PersistenceContext.GetProxy(entityKey);
			}
			return null;
		}
		
		private void SetReadOnly(bool readOnly)
		{
			IEntityPersister persister = _session.Factory.GetEntityPersister(_entityName);

			if (!persister.IsMutable && !readOnly)
			{
				throw new InvalidOperationException("cannot make proxies for immutable entities modifiable");
			}
			
			this.readOnly = readOnly;
		
			if (initialized)
			{
				EntityKey key = GenerateEntityKeyOrNull(_id, _session, _entityName);
				if (key != null && _session.PersistenceContext.ContainsEntity(key))
				{
					_session.PersistenceContext.SetReadOnly(_target, readOnly);
				}
			}
		}
	}
}
