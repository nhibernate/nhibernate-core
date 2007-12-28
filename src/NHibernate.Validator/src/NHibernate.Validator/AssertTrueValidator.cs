namespace NHibernate.Validator
{
	using System;

	public class AssertTrueValidator : Validator<AssertTrueAttribute>
	{
		public override bool IsValid(Object value)
		{
			return (bool) value;
		}

		public override void Initialize(AssertTrueAttribute parameters)
		{
		}
	}
}