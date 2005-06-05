using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH257
{
	[Serializable]
	public abstract class Party
	{
		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}
