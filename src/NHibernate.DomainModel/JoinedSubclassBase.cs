using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for JoinedSubclassBase.
	/// </summary>
	public class JoinedSubclassBase
	{
		private int id;
		private long testLong;
		private string testString;
		private System.DateTime testDate;

		public JoinedSubclassBase()
		{
		}


		public int Id {
			get {return this.id;}
			set {this.id = value;}
		}

		public long TestLong {
			get {return this.testLong;}
			set {this.testLong = value;}
		}

		public string TestString {
			get {return this.testString;}
			set {this.testString = value;}
		}

		public System.DateTime TestDateTime {
			get {return this.testDate;}
			set {this.testDate = value;}
		}


	}
}
