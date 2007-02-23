using System;

namespace NHibernate.Test.Subclass
{
	/// <summary>
	/// Summary description for JoinedSubclassOne.
	/// </summary>
	public class SubclassOne : SubclassBase
	{
		private long _oneTestLong;

		public SubclassOne()
		{
		}

		public long OneTestLong
		{
			get { return _oneTestLong; }
			set { _oneTestLong = value; }
		}
	}
}