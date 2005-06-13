using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH309
{
	/// <summary>
	/// Summary description for Menu.
	/// </summary>
	public class Menu
	{
		private int _id;
		private string _name;
		private IList _nodes;

		public int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public IList Nodes
		{
			get 
			{
				if (this._nodes == null)
				{
					this._nodes = new ArrayList();
				}
				return this._nodes; 
			}
			set { this._nodes = value; }
		}

		public Menu()
		{
			this._id = -1;
		}
	}
}
