using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Node.
	/// </summary>
	public class Node
	{
		private string id;

		private IDictionary previousNodes;
		private IDictionary destinationNodes;

		private Node()
		{
		}

		public Node(string id) 
		{
			this.destinationNodes = new Hashtable();
			this.previousNodes = new Hashtable();
			this.id = id;
		}

		public string Id 
		{
			get { return id; }
			set { id = value; }
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
		public IDictionary PreviousNodes 
		{
			get { return previousNodes; }
			set { previousNodes = value; }
		}

		private void AddPreviousNode(Node previousNode) 
		{
			PreviousNodes.Add(previousNode, null);
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
		public IDictionary DestinationNodes
		{
			get { return destinationNodes; }
			set { destinationNodes = value; }
		}

		/// <summary>
		/// This is the only way to hook nodes together right now.
		/// </summary>
		/// <param name="node">A Node this Node can go to.</param>
		public void AddDestinationNode(Node destinationNode) 
		{
			DestinationNodes.Add(destinationNode, null);

			// let the destinationNode know that it can be one of the
			// previous Nodes was this Node
			destinationNode.AddPreviousNode(this);
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Node rhs = obj as Node;
			if(rhs==null) return false;

			return id.Equals(rhs.Id);
		}

	}
}
