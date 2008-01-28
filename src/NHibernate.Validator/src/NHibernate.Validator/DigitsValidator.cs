namespace NHibernate.Validator
{
	public class DigitsValidator : Validator<DigitsAttribute>
	{
		/// <summary>
		/// does the object/element pass the constraints
		/// </summary>
		/// <param name="value">Object to be validated</param>
		/// <returns>if the instance is valid</returns>
		public override bool IsValid(object value)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Take the annotations values and Initialize the Validator
		/// </summary>
		/// <param name="parameters">parameters</param>
		public override void Initialize(DigitsAttribute parameters)
		{
			throw new System.NotImplementedException();
		}
	}
}