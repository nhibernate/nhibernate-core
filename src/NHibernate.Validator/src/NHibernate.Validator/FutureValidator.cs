namespace NHibernate.Validator
{
	using System;

	[Serializable]
	public class FutureValidator : IValidator<FutureAttribute>
	{
		public bool IsValid(object value)
		{
			if (value == null) return true;

			if(value is DateTime)
			{
				DateTime date = (DateTime) value;
				return date.CompareTo(DateTime.Now) >= 0;
			}

			//TODO: Add support to System.Globalization.Calendar ?
			//if(value is Calendar)
			//{
			//}

			return false;
		}

		public void Initialize(Attribute parameters)
		{
		}
	}
}