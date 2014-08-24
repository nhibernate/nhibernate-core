namespace NHibernate.Proxy
{
	/// <summary>
	/// A marker interface so NHibernate can know if it is dealing with
	/// an object that is a Proxy. 
	/// </summary>
	/// <remarks>
	/// <para>
	/// This interface should not be implemented by anything other than
	/// the Dynamically generated Proxy.  If it is implemented by a class then
	/// NHibernate will think that class is a Proxy and will not work.
	/// </para> 
	/// <para>
	/// It has to be public scope because
	/// the Proxies are created in a separate DLL than NHibernate. 
	/// </para> 
	/// </remarks>
	public interface INHibernateProxy
	{
		/// <summary> Get the underlying lazy initialization handler. </summary>
		ILazyInitializer HibernateLazyInitializer { get;}
	}
}
