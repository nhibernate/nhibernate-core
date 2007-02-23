using System;

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