using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for JoinedSubclassBase.
	/// </summary>
	public class JoinedSubclassBase
	{
		private int _id;
		private long _testLong;
		private string _testString;
		private System.DateTime _testDate;

		public JoinedSubclassBase()
		{
		}

		public int Id {
			get {return _id;}
			set {_id = value;}
		}

		public long TestLong {
			get {return _testLong;}
			set {_testLong = value;}
		}

		public string TestString {
			get {return _testString;}
			set {_testString = value;}
		}

		public System.DateTime TestDateTime {
			get {return _testDate;}
			set {_testDate = value;}
		}
	}
}
