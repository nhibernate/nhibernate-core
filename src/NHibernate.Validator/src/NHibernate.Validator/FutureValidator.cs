namespace NHibernate.Validator
{
	using System;

	[Serializable]
	public class FutureValidator : Validator<FutureAttribute>
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
				return date.CompareTo(DateTime.Now) >= 0;
			}

			//TODO: Add support to System.Globalization.Calendar ?
			//if(value is Calendar)
			//{
			//}

			return false;
		}

		public override void Initialize(FutureAttribute parameters)
		{
		}
	}
}