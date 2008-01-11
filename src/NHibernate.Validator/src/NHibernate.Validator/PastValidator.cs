namespace NHibernate.Validator
{
	using System;

	public class PastValidator : Validator<PastAttribute>
	{
		public override bool IsValid(object value)
		{
			if (value == null)
			{
				return true;
			}

			if (value is DateTime)
			{
				DateTime date = (DateTime) value;
				return date.CompareTo(DateTime.Now) < 0;
			}

			//TODO: Add support to System.Globalization.Calendar ?
			//if(value is Calendar)
			//{
			//}

			return false;
		}

		public override void Initialize(PastAttribute parameters)
		{
		}
	}
}