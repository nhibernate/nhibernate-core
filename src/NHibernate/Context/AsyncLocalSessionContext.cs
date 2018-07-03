using System;
using System.Threading;
using NHibernate.Engine;

namespace NHibernate.Context
{
	// Session contextes are serializable while not actually serializing any data. Others contextes just retrieve it back
	// from their context, if it does still live when/where they are de-serialized. For having that with AsyncLocal,
	// we would need to store it as static, and then we need to use a MapBasedSessionContext.
	// But this would cause bindings operations done in inner flows to be potentially propagated to outer flows, depending
	// on which flow has initialized the map. This is undesirable.
	// So current implementation just loses its context in case of serialization, since AsyncLocal is not serializable.
	// Another option would be to change MapBasedSessionContext for recreating a new dictionary from the
	// previous one at each change, essentially using those dictionaries as immutable objects.
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for current asynchronous flow.
	/// </summary>
	[Serializable]
	public class AsyncLocalSessionContext : CurrentSessionContext
	{
		private readonly AsyncLocal<ISession> _session = new AsyncLocal<ISession>();

		// Since v5.2
		[Obsolete("This constructor has no more usages and will be removed in a future version")]
		public AsyncLocalSessionContext(ISessionFactoryImplementor factory) { }

		public AsyncLocalSessionContext() { }

		protected override ISession Session
		{
			get => _session.Value;
			set => _session.Value = value;
		}
	}
}
