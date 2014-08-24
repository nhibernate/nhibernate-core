using System;

namespace NHibernate.Test.Unconstrained
{
	public class SimplyB
	{
		private int _id = 0;

		public SimplyB()
		{
		}

		public SimplyB(int id)
			: this()
		{
			_id = id;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}