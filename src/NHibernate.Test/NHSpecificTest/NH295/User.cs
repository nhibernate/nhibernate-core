using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH295
{
	[Serializable]
	public class User : Party
	{
		private ISet _groups = new HashedSet();

		internal User() : this("")
		{
		}

		internal User(string name)
			: base(name)
		{
		}

		public ISet Groups
		{
			get { return _groups; }
			set { _groups = value; }
		}
	}
}