using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	public class Node
	{
		private string _id;

		private Iesi.Collections.ISet _previousNodes;
		private Iesi.Collections.ISet _destinationNodes;

		private Node()
		{
		}

		public Node(string id) 
		{
			_destinationNodes = new Iesi.Collections.HashedSet();
			_previousNodes = new Iesi.Collections.HashedSet();
			_id = id;
		}

		public string Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// The Nodes that lead into this Node.
		/// </summary>
		/// <remarks>
		/// I would not recommend that mapping of set be made public because
		/// under the scene they rely on Dictionaries, but this is in here for 
		/// testing.
		/// 
		/// Any modifications to the "inverse" side should not be persisted - unless
		/// the modifications are also made to the non-inverse side.
		/// </remarks>
		public Iesi.Collections.ISet PreviousNodes 
		{
			get { return _previousNodes; }
			set { _previousNodes = value; }
		}

		private void AddPreviousNode(Node previousNode) 
		{
			PreviousNodes.Add( previousNode );
		}

		private void RemovePreviousNode(Node previousNode) 
		{
			PreviousNodes.Remove(previousNode);
		}

		/// <summary>
		/// The Nodes this Node can go To.
		/// </summary>
		/// <remarks>
		/// I would not recommend that mapping of set be made public because
		/// under the scene they rely on Dictionaries, but this is in here for 
		/// testing.  The DestinationNodes is the Property that controls which 
		/// modifications get persisted.
		/// </remarks>
		public Iesi.Collections.ISet DestinationNodes
		{
			get { return _destinationNodes; }
			set { _destinationNodes = value; }
		}

		/// <summary>
		/// This is the only way to hook nodes together right now.
		/// </summary>
		/// <param name="node">A Node this Node can go to.</param>
		public void AddDestinationNode(Node destinationNode) 
		{
			DestinationNodes.Add( destinationNode );

			// let the destinationNode know that it can be one of the
			// previous Nodes was this Node
			destinationNode.AddPreviousNode(this);
		}

		public void RemoveDestinationNode(Node destinationNode) 
		{
			DestinationNodes.Remove(destinationNode);
			destinationNode.RemovePreviousNode(this);
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Node rhs = obj as Node;
			if(rhs==null) return false;

			return _id.Equals(rhs.Id);
		}

	}
}
