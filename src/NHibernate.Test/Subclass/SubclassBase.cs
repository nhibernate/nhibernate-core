using System;

namespace NHibernate.Test.Subclass
{
	/// <summary>
	/// Summary description for SubclassBase.
	/// </summary>
	public class SubclassBase
	{
		private int _id = 0;
		private long _testLong;
		private string _testString;
		private System.DateTime _testDate;

		public SubclassBase()
		{
		}

		public int Id 
		{
			get { return _id; }
		}

		public long TestLong 
		{
			get { return _testLong; }
			set { _testLong = value; }
		}

		public string TestString 
		{
			get { return _testString; }
			set { _testString = value; }
		}

		public System.DateTime TestDateTime 
		{
			get { return _testDate; }
			set { _testDate = value; }
		}
	}
}
