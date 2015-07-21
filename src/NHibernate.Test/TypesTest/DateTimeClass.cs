using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for GuidClass.
	/// </summary>
	public class DateTimeClass
	{
		private int _id;
		private DateTime? _utcDateTimeValue;
		private DateTime? _localDateTimeValue;
		public DateTimeClass()
		{
			NormalDateTimeValue = DateTime.Today;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public DateTime? UtcDateTimeValue
		{
			get { return _utcDateTimeValue; }
			set { _utcDateTimeValue = value; }
		}

		public DateTime? LocalDateTimeValue
		{
			get { return _localDateTimeValue; }
			set { _localDateTimeValue = value; }
		}

		public DateTime NormalDateTimeValue { get; set; }
	}
}
