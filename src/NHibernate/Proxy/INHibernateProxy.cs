using System;
using System.Runtime.Serialization;

namespace NHibernate.Proxy
{
	/// <summary>
	/// A marker interface so NHibernate can know if it is dealing with
	/// an object that is a Proxy.  All NHibernate Proxies will be Serializable.
	/// This does not mean that all Mapped Classes have to be Serializable.
	/// </summary>
	/// <remarks>
	/// This interface should not be implemented by anything other than
	/// the Dynamically generated Proxy.  It has to be public scope because
	/// the Proxies are created in a seperate DLL than NHibernate. 
	/// </remarks>
	public interface INHibernateProxy : ISerializable	
	{
	}
}
