using System;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each thread using the [<see cref="ThreadStaticAttribute"/>].
	/// To avoid if there are two session factories in the same thread.
	/// </summary>
	[Serializable]
	public class ThreadStaticSessionContext : CurrentSessionContext
	{
		[ThreadStatic]
		private static IDictionary _map;
                private readonly ISessionFactoryImplementor _factory;

		public ThreadStaticSessionContext(Engine.ISessionFactoryImplementor factory)
		{
			_factory = factory;
		}

		/// <summary> Gets or sets the currently bound session. </summary>
		protected override ISession Session
		{
			get 
			{ 
				if (_map == null)
                		{
                    			return null;
                		}
                		else
                		{
                			return _map[_factory] as ISession;
                		}
			}
			set 
			{ 
				if (_map == null)
                		{
                			_map = new Hashtable();
                		}
                		_map[_factory] = value;
			}
		}
	}
}
