using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1217
{
	public class DomainBase : IDomainBase
	{
		private Int32 _id;


		private Int32 _versionNumber;

		#region IDomainBase Members

		public virtual Int32 Id
		{
			get { return _id; }
			set { _id = value; }
		}


		public virtual Int32 VersionNumber
		{
			get { return _versionNumber; }
			set { _versionNumber = value; }
		}

		#endregion
	}


	public class Root : DomainBase, IRoot
	{
		private String _name;


		private IList<INode> _nodes;

		#region IRoot Members

		public virtual String Name
		{
			get { return _name; }
			set { _name = value; }
		}


		public virtual IList<INode> Nodes
		{
			get
			{
				if (_nodes == null) _nodes = new List<INode>();
				return _nodes;
			}
			set { _nodes = value; }
		}


		public virtual INode AddNode(string label)
		{
			INode result = new Node();
			result.Label = label;
			result.Root = this;
			Nodes.Add(result);

			return result;
		}

		public virtual IEdge AddLink(INode from, INode to, string label)
		{
			IEdge result = new Edge();
			result.FromNode = from;
			result.ToNode = to;
			result.Label = label;

			from.FromEdges.Add(result);
			to.ToEdges.Add(result);

			return result;
		}

		#endregion
	}

	public class Node : DomainBase, INode
	{
		private ISet<IEdge> _fromEdges;
		private String _label;
		private IRoot _root;
		private ISet<IEdge> _toEdges;

		#region INode Members

		public virtual IRoot Root
		{
			get { return _root; }
			set { _root = value; }
		}


		public virtual String Label
		{
			get { return _label; }
			set { _label = value; }
		}


		public virtual ISet<IEdge> FromEdges
		{
			get
			{
				if (_fromEdges == null) _fromEdges = new HashSet<IEdge>();
				return _fromEdges;
			}
			set { _fromEdges = value; }
		}


		public virtual ISet<IEdge> ToEdges
		{
			get
			{
				if (_toEdges == null) _toEdges = new HashSet<IEdge>();
				return _toEdges;
			}
			set { _toEdges = value; }
		}

		#endregion
	}


	public class Edge : DomainBase, IEdge
	{
		private INode _fromNode;
		private String _label;
		private INode _toNode;

		#region IEdge Members

		public virtual String Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public virtual INode FromNode
		{
			get { return _fromNode; }
			set { _fromNode = value; }
		}


		public virtual INode ToNode
		{
			get { return _toNode; }
			set { _toNode = value; }
		}

		#endregion
	}
}