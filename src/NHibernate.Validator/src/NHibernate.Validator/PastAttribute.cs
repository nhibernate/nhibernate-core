namespace NHibernate.Validator
{
	using System;

	/// <summary>
	/// Check that a Date representation apply in the past
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(PastValidator))]
	public class PastAttribute : Attribute
	{
		private string message = "{validator.past}";

		public string Message {
			get { return message; }
			set { message = value; }
		}
	}
}