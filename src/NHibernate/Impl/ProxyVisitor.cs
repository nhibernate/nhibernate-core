using System;
using System.Collections;

using NHibernate.Type;

namespace NHibernate.Impl
{
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
