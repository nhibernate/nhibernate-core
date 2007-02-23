using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH317
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	[Serializable]
	public class Node
	{
		private int _id;
		private string _name;
		private Node _parentNode;
		private IList _childNodes;

		public virtual int Id
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public virtual Node ParentNode
		{
			get { return this._parentNode; }
			set { this._parentNode = value; }
		}

		public virtual IList ChildNodes
		{
			get { return this._childNodes; }
			set { this._childNodes = value; }
		}

		public Node()
		{
			this._id = -1;
		}
	}
}