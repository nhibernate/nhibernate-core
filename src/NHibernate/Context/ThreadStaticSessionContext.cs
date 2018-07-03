using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each thread using the [<see cref="ThreadStaticAttribute"/>].
	/// </summary>
	[Serializable]
	public class ThreadStaticSessionContext : MapBasedSessionContext
	{
		[ThreadStatic]
		private static IDictionary _map;

		// Since v5.2
		[Obsolete("This constructor has no more usages and will be removed in a future version")]
		public ThreadStaticSessionContext(ISessionFactoryImplementor factory) : base (factory) { }

		public ThreadStaticSessionContext() { }

		protected override IDictionary GetMap()
		{
			return _map;
		}

		protected override void SetMap(IDictionary value)
		{
			_map = value;
		}
	}
}
