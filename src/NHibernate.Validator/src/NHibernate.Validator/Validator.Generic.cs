namespace NHibernate.Validator
{
	using System;

	/// <summary>
	/// Represent a Validator for a Type. In order to get your own validator
	/// you can extend this class.
	/// </summary>
	/// <typeparam name="A"></typeparam>
	public abstract class Validator<A> : IValidator<A> where A : Attribute
	{
		/// <summary>
		/// does the object/element pass the constraints
		/// </summary>
		/// <param name="value">Object to be validated</param>
		/// <returns>if the instance is valid</returns>
		public abstract bool IsValid(object value);

		/// <summary>
		/// Take the annotations values and Initialize the Validator
		/// </summary>
		/// <param name="parameters">parameters</param>
		public abstract void Initialize(A parameters);

		/// <summary>
		/// Take the annotations values and Initialize the Validator
		/// </summary>
		/// <param name="parameters">parameters</param>
		public void Initialize(Attribute parameters)
		{
			Initialize((A) parameters);
		}


	}
}