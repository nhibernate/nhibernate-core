namespace NHibernate.Validator
{
	using System;

	/// <summary>
	/// A constraint validator for a particular annotation
	/// </summary>
	public interface IValidator<A> : IValidator where A : Attribute
	{
		/// <summary>
		/// does the object/element pass the constraints
		/// </summary>
		/// <param name="value">Object to be validated</param>
		/// <returns>if the instance is valid</returns>
		new bool IsValid(Object value);

		/// <summary>
		/// Take the annotations values and Initialize the Validator
		/// </summary>
		/// <param name="parameters">parameters</param>
		void Initialize(A parameters);
	}
}