using System;

namespace NHibernate.Test.Unconstrained
{
	public class SimplyA
	{
		private string _name;
		private SimplyB _simplyB;

		public SimplyA()
		{
		}

		public SimplyA(string name)
			: this()
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public SimplyB SimplyB
		{
			get { return _simplyB; }
			set { _simplyB = value; }
		}
	}
}