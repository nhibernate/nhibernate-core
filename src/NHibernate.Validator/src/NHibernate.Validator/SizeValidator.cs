namespace NHibernate.Validator
{
	using System;
	using System.Collections;

	public class SizeValidator : IValidator<SizeAttribute>
	{
		private int min;
		private int max;


		public bool IsValid(object value)
		{
			ICollection collection = value as ICollection;

			if(collection == null) return true;

			return collection.Count >= min && collection.Count <= max;
		}

		public void Initialize(Attribute parameters)
		{
			SizeAttribute @param = (SizeAttribute) parameters;

			min = @param.Min;
			max = @param.Max;
		}
	}
}