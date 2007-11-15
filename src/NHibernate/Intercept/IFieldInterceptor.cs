using NHibernate.Engine;

namespace NHibernate.Intercept
{
	/// <summary> Contract for field interception handlers. </summary>
	public interface IFieldInterceptor
	{
		/// <summary> Is the entity considered dirty? </summary>
		/// <value> True if the entity is dirty; otherwise false. </value>
		bool IsDirty { get;}

		/// <summary> Use to associate the entity to which we are bound to the given session. </summary>
		/// <param name="session">The session to which we are now associated. </param>
		void SetSession(ISessionImplementor session);

		/// <summary> Is the entity to which we are bound completely initialized? </summary>
		bool IsInitialized { get;}

		/// <summary> The the given field initialized for the entity to which we are bound? </summary>
		/// <param name="field">The name of the field to check </param>
		/// <returns> True if the given field is initialized; otherwise false.</returns>
		bool IsInitializedField(string field);

		/// <summary> Forcefully mark the entity as being dirty.</summary>
		void MarkDirty();

		/// <summary> Clear the internal dirty flag.</summary>
		void ClearDirty();
	}
}