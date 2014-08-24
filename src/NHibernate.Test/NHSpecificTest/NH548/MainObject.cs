using System;

namespace NHibernate.Test.NHSpecificTest.NH548
{
	public class MainObject
	{
#pragma warning disable 649
		private int _id;
		private string _name;
		private ComponentObject _component;
#pragma warning restore 649

		public MainObject()
		{
			_component = new ComponentObject(this);
		}

		public int ID
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public ComponentObject Component
		{
			get { return _component; }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}