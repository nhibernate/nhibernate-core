using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for JoinedSubclassOne.
	/// </summary>
	public class JoinedSubclassOne: JoinedSubclassBase
	{
		private long oneTestLong;

		public JoinedSubclassOne() {}

		public long OneTestLong {
			get {return this.oneTestLong;}
			set {this.oneTestLong = value;}
		}

	}
}
