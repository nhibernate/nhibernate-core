namespace NHibernate.Classic
{
	/// <summary>
	/// Provides callbacks from the <see cref="ISession" /> to the persistent object. Persistent classes may
	/// implement this interface but they are not required to.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="OnSave" />, <see cref="OnDelete" />, and <see cref="OnUpdate" /> are intended to be used
	/// to cascade saves and deletions of dependent objects. This is an alternative to declaring cascaded
	/// operations in the mapping file.
	/// </para>
	/// <para>
	/// <see cref="OnLoad" /> may be used to initialize transient properties of the object from its persistent
	/// state. It may <em>not</em> be used to load dependent objects since the <see cref="ISession" /> interface
	/// may not be invoked from inside this method.
	/// </para>
	/// <para>
	/// A further intended usage of <see cref="OnLoad" />, <see cref="OnSave" />, and <see cref="OnUpdate" />
	/// is to store a reference to the <see cref="ISession" /> for later use.
	/// </para>
	/// <para>
	/// If <see cref="OnSave" />, <see cref="OnUpdate" />, or <see cref="OnDelete" /> return
	/// <see cref="LifecycleVeto.Veto" />, the operation is silently vetoed. If a <see cref="CallbackException" />
	/// is thrown, the operation is vetoed and the exception is passed back to the application.
	/// </para>
	/// <para>
	/// Note that <see cref="OnSave" /> is called after an identifier is assigned to the object, except when
	/// <c>identity</c> key generation is used.
	/// </para>
	/// </remarks>
	public interface ILifecycle
	{
		/// <summary>
		/// Called when an entity is saved
		/// </summary>
		/// <param name="s">The session</param>
		/// <returns>If we should veto the save</returns>
		LifecycleVeto OnSave(ISession s);

		/// <summary>
		/// Called when an entity is passed to <see cref="ISession.Update(object)"/>.
		/// </summary>
		/// <param name="s">The session</param>
		/// <returns>A <see cref="LifecycleVeto" /> value indicating whether the operation
		/// should be vetoed or allowed to proceed.</returns>
		/// <remarks>
		/// This method is <em>not</em> called every time the object's state is
		/// persisted during a flush.
		/// </remarks>
		LifecycleVeto OnUpdate(ISession s);

		/// <summary>
		/// Called when an entity is deleted
		/// </summary>
		/// <param name="s">The session</param>
		/// <returns>A <see cref="LifecycleVeto" /> value indicating whether the operation
		/// should be vetoed or allowed to proceed.</returns>
		LifecycleVeto OnDelete(ISession s);

		/// <summary>
		/// Called after an entity is loaded. 
		/// </summary>
		/// <remarks>
		/// <note>It is illegal to access the <see cref="ISession" /> from inside this method.</note>. 
		/// However, the object may keep a reference to the session for later use
		/// </remarks>
		/// <param name="s">The session</param>
		/// <param name="id">The identifier</param>
		void OnLoad(ISession s, object id);
	}

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