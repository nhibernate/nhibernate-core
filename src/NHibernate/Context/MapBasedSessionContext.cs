using System.Collections;
using System.Collections.Concurrent;
using NHibernate.Engine;

namespace NHibernate.Context
{
	public abstract class MapBasedSessionContext : CurrentSessionContext
	{
		private readonly ISessionFactoryImplementor _factory;

		// Must be static, different instances of MapBasedSessionContext may have to yield the same map.
		private static readonly object _locker = new object();

		protected MapBasedSessionContext(ISessionFactoryImplementor factory)
		{
			_factory = factory;
		}

		/// <summary>
		/// Gets or sets the currently bound session.
		/// </summary>
		protected override ISession Session
		{
			get
			{
				ISession value = null;
				GetConcreteMap()?.TryGetValue(_factory, out value);
				// We want null if no value was there, no need to explicitly handle false outcome of TryGetValue.
				return value;
			}
			set
			{
				var map = GetConcreteMap();
				if (map == null)
				{
					// Double check locking. Cannot use a Lazy<T> for such a semantic: the map can be bound
					// to some execution context through SetMap/GetMap, a Lazy<T> would defeat those methods purpose.
					lock (_locker)
					{
						map = GetConcreteMap();
						if (map == null)
						{
							map = new ConcurrentDictionary<ISessionFactoryImplementor, ISession>();
							SetMap(map);
						}
					}
				}
				map[_factory] = value;
			}
		}

		private ConcurrentDictionary<ISessionFactoryImplementor, ISession> GetConcreteMap()
		{
			return (ConcurrentDictionary<ISessionFactoryImplementor, ISession>)GetMap();
		}

		/// <summary>
		/// Get the dictionary mapping session factory to its current session. Yield <c>null</c> if none have been set.
		/// </summary>
		protected abstract IDictionary GetMap();

		/// <summary>
		/// Set the map mapping session factory to its current session.
		/// </summary>
		protected abstract void SetMap(IDictionary value);
	}
}