using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicTime.
	/// </summary>
	public class BasicTime
	{
		private int _id;
		private DateTime _timeValue;
		private DateTime[] _timeArray;

		public BasicTime()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public DateTime TimeValue
		{
			get { return _timeValue; }
			set { _timeValue = value; }
		}

		public DateTime[] TimeArray
		{
			get { return _timeArray; }
			set { _timeArray = value; }
		}
	}
}