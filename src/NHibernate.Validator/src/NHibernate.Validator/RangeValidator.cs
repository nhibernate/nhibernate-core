namespace NHibernate.Validator
{
	using System;
	using Mapping;

	public class RangeValidator : IValidator<RangeAttribute>, IPropertyConstraint
	{
		private int min;
		private int max;

		public void Initialize(Attribute parameters) 
		{
			RangeAttribute @param = (RangeAttribute) parameters;

			max = @param.Max;
			min = @param.Min;
		}

		public bool IsValid(object value)
		{
			if (value == null) 
			{
				return true;
			}

			if (value is string) 
			{
				try 
				{
					return	Convert.ToDecimal(value) >= Convert.ToDecimal(min) &&
							Convert.ToDecimal(value) <= Convert.ToDecimal(max);
				} 
				catch (FormatException fe) 
				{
					return false;
				}
			} 
			else if (value is decimal) 
			{
				return	Convert.ToDecimal(value) >= Convert.ToDecimal(min) &&
						Convert.ToDecimal(value) <= Convert.ToDecimal(max);
			} 
			else if (value is Int32) 
			{
				return	Convert.ToInt32(value) >= Convert.ToInt32(min) &&
						Convert.ToInt32(value) <= Convert.ToInt32(max);
			} 
			else if (value is Int64) 
			{
				return Convert.ToInt64(value) >= Convert.ToInt64(min) &&
						Convert.ToInt64(value) <= Convert.ToInt64(max);
			}

			return false;
		}

		public void Apply(Property property)
		{
			new NotImplementedException();

			//Column col = (Column)property.getColumnIterator().next();
			//String check = "";
			//if (min != Long.MIN_VALUE) check += col.getName() + ">=" + min;
			//if (max != Long.MAX_VALUE && min != Long.MIN_VALUE) check += " and ";
			//if (max != Long.MAX_VALUE) check += col.getName() + "<=" + max;
			//col.setCheckConstraint(check);
		}
	}
}