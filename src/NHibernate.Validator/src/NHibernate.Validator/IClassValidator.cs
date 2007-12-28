namespace NHibernate.Validator
{
	using Mapping;

	public interface IClassValidator
	{
		/// <summary>
		/// Return true if this <see cref="ClassValidator"/> contains rules for apply, false in other case. 
		/// </summary>
		bool HasValidationRules { get; }

		/// <summary>
		/// apply constraints on a bean instance and return all the failures.
		/// if <see cref="bean"/> is null, an empty array is returned 
		/// </summary>
		/// <param name="bean">object to apply the constraints</param>
		/// <returns></returns>
		InvalidValue[] GetInvalidValues(object bean);

		/// <summary>
		/// Assert a valid Object. A <see cref="InvalidStateException"/> 
		/// will be throw in a Invalid state.
		/// </summary>
		/// <param name="bean">Object to be asserted</param>
		void AssertValid(object bean);

		/// <summary>
		/// Apply constraints of a particular property value of a bean type and return all the failures.
		/// The InvalidValue objects returns return null for InvalidValue#getBean() and InvalidValue#getRootBean()
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		InvalidValue[] GetPotentialInvalidValues(string propertyName, object value);

		/// <summary>
		/// Apply the registred constraints rules on the hibernate metadata (to be applied on DB schema)
		/// </summary>
		/// <param name="persistentClass">hibernate metadata</param>
		void Apply(PersistentClass persistentClass);
	}
}