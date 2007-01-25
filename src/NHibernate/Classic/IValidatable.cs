namespace NHibernate.Classic
{
	/// <summary>
	/// Implemented by persistent classes with invariants that must be checked before inserting
	/// into or updating the database
	/// </summary>
	public interface IValidatable
	{
		/// <summary>
		/// Validate the state of the object before persisting it. If a violation occurs,
		/// throw a <c>ValidationFailure</c>. This method must not change the state of the object
		/// by side-effect.
		/// </summary>
		void Validate();
	}
}