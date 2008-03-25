using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH257
{
	[Serializable]
	public class User : Party
	{
		private ISet _groups = new HashedSet();

		public ISet Groups
		{
			get { return _groups; }
			set { _groups = value; }
		}
	}
}