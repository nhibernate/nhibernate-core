namespace NHibernate.Validator.Tests
{
	using System;

	[AttributeUsage(AttributeTargets.Class)]
	[ValidatorClass(typeof(AssertAnimalValidator))]
	public class AssertAnimalAttribute : Attribute
	{
		private string message = "not a animal";

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}

	public class AssertAnimalValidator : IValidator<AssertAnimalAttribute>
	{
		public bool IsValid(object value)
		{
			return value is Animal;
		}

		public void Initialize(Attribute parameters)
		{
			
		}
	}
}