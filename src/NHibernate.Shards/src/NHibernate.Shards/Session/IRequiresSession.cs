using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Interface describing an object that can have a Session set on it.  This
	/// is designed to be used in conjunction with stateful interceptors.
	/// <seealso cref="StatefulInterceptorFactory"/>
	/// </summary>
    public interface IRequiresSession
    {
		void SetSession(ISession session);
    }
}
