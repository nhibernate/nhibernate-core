using System;
using System.Collections;
using System.Threading;
using NHibernate.Engine;
#if NETFX
using System.Runtime.Remoting.Messaging;
#endif

namespace NHibernate.Context
{
#pragma warning disable CS1574
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each <see cref="System.Runtime.Remoting.Messaging.CallContext"/>.
	/// Uses <see cref="AsyncLocal{T}"/> instead if run under .NET Core/.NET Standard.
	/// <remarks>
	/// <para>Not recommended for .NET 2.0 web applications.</para>
	/// </remarks>
	/// </summary>
#pragma warning restore CS1574
	[Serializable]
	public class CallSessionContext : MapBasedSessionContext
	{
		#if NETFX
		private const string SessionFactoryMapKey = "NHibernate.Context.CallSessionContext.SessionFactoryMapKey";
		#else
		private static readonly AsyncLocal<IDictionary> SessionFactoryMap = new AsyncLocal<IDictionary>();
		#endif

		public CallSessionContext(ISessionFactoryImplementor factory) : base(factory)
		{
		}

		/// <summary>
		/// The key is the session factory and the value is the bound session.
		/// </summary>
		protected override void SetMap(IDictionary value)
		{
#if NETFX
			CallContext.SetData(SessionFactoryMapKey, value);
#else
			SessionFactoryMap.Value = value;
#endif
		}

		/// <summary>
		/// The key is the session factory and the value is the bound session.
		/// </summary>
		protected override IDictionary GetMap()
		{
#if NETFX
			return CallContext.GetData(SessionFactoryMapKey) as IDictionary;
#else
			return SessionFactoryMap.Value;
#endif
		}
	}
}
