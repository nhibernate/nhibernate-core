using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for JoinedSubclassOne.
	/// </summary>
	public class JoinedSubclassOne: JoinedSubclassBase
	{
		private long _oneTestLong;

		public JoinedSubclassOne() {}

		public long OneTestLong {
			get {return _oneTestLong;}
			set {_oneTestLong = value;}
		}

	}
}
