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

		public ThreadStaticSessionContext(ISessionFactoryImplementor factory) : base(factory) { }

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
