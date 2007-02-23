using System;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Reassociates uninitialized Proxies with the Session.
	/// </summary>
	internal abstract class ProxyVisitor : AbstractVisitor
	{
		public ProxyVisitor(SessionImpl session) : base(session)
		{
		}

		protected override object ProcessEntity(object value, EntityType entityType)
		{
			if (value != null)
			{
				Session.ReassociateIfUninitializedProxy(value);
				// if it is an initialized proxy, let cascade 
				// handle it later on
			}

			return null;
		}
	}
}