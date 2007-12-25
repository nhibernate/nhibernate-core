namespace NHibernate.Validator
{
	using System;

	public class AssertTrueValidator : IValidator<AssertTrueAttribute>
	{
		public bool IsValid(Object value)
		{
			return (bool) value;
		}

		public void Initialize(Attribute parameters)
		{
		}
	}
}