namespace NHibernate
{
	/// <summary>
	/// Provides callbacks from the <c>ISession</c> to the persistent object. Persistent classes may
	/// implement this interface but they are not required to.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <c>OnSave()</c>, <c>OnDelete()</c>, and <c>OnUpdate()</c> are intended to be used to cascade
	/// saves and deletions of dependent objects. This is an alternative to declaring cascaded operations
	/// in the mapping file.
	/// </para>
	/// <para>
	/// <c>OnLoad()</c> may be used to initialize transient properties of the object from its persistent
	/// state. It may <b>not</b> be used to load dependent objects since the <c>ISession</c> interface
	/// may not be invoked from inside this method
	/// </para>
	/// <para>
	/// A further intended usage of <c>OnLoad()</c>, <c>OnSave()</c>, and <c>OnUpdate()</c> is to store
	/// a reference to the <c>ISession</c> for later use.
	/// </para>
	/// <para>
	/// If <c>OnSave()</c>, <c>OnUpdate()</c>, or <c>OnDelete</c> return <c>Veto</c>, the operation is
	/// silently vetoed. If a <c>CallbackException</c> is thrown, the operation is vetoed and the
	/// exception is passed back to the application
	/// </para>
	/// <para>
	/// Note that <c>OnSave()</c> is called after an identifier is assigned to the object, exception when
	/// native key generation is used.
	/// </para>
	/// </remarks>
	public interface ILifecycle
	{
		/// <summary>
		/// Called when an entity is saved
		/// </summary>
		/// <param name="s">The session</param>
		/// <returns>If we should veto the save</returns>
		LifecycleVeto OnSave( ISession s );

		/// <summary>
		/// Called when an entity is passed to <c>ISession.Update()</c>.
		/// </summary>
		/// <remarks>
		/// This method is <em>not</em> called every time the object's state is
		/// persisted during a flush.
		/// </remarks>
		/// <param name="s">The session</param>
		/// <returns>If we should veto the update</returns>
		LifecycleVeto OnUpdate( ISession s );

		/// <summary>
		/// Called when an entity is deleted
		/// </summary>
		/// <param name="s">The session</param>
		/// <returns>If we should veto the delete</returns>
		LifecycleVeto OnDelete( ISession s );

		/// <summary>
		/// Called after an entity is loaded. 
		/// </summary>
		/// <remarks>
		/// <em>It is illegal to access the <c>ISession</c> from inside this method.</em>. 
		/// However, the object may keep a reference to the session for later use
		/// </remarks>
		/// <param name="s">The session</param>
		/// <param name="id">The identifier</param>
		void OnLoad( ISession s, object id );
	}

	/// <summary></summary>
	public enum LifecycleVeto
	{
		/// <summary>
		/// Veto the action
		/// </summary>
		Veto,
		/// <summary>
		/// Accept the action
		/// </summary>
		NoVeto
	}
}