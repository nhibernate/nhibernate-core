namespace NHibernate.Validator
{
	using System;
	using Mapping;

	public class LengthValidator : IValidator<LengthAttribute>, IPropertyConstraint
	{
		private int min;
		private int max;

		public bool IsValid(object value)
		{
			if(value == null) return true;
			if (!(value is string)) return false;

			string @string = (string) value;
			int length = @string.Length;

			return length >= min && length <= max;
		}


		public void Initialize(LengthAttribute parameters)
		{
			min = parameters.Min;
			max = parameters.Max;
		}

		public void Initialize(Attribute parameters) 
		{
			Initialize((LengthAttribute)parameters);
		}

		public void apply(Property property)
		{
		}
	}
}