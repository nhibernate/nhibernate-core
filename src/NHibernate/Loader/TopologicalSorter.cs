using System;

namespace NHibernate.Loader
{
	class TopologicalSorter
	{
		sealed class Node
		{
			public int Index { get; private set; }
			public int SuccessorCount { get; private set; }
			public bool Eliminated { get; private set; }
			System.Action _onEliminate;

			public Node(int index)
			{
				Index = index;
				SuccessorCount = 0;
				Eliminated = false;
				_onEliminate = null;
			}

			public void RegisterSuccessor(Node successor)
			{
				SuccessorCount++;
				successor._onEliminate += () => SuccessorCount--;
			}

			public void Eliminate()
			{
				if (_onEliminate != null)
				{
					_onEliminate();
					_onEliminate = null;
				}
				Eliminated = true;
			}
		}

		readonly Node[] _nodes;
		int _nodeCount;

		public TopologicalSorter(int size)
		{
			_nodes = new Node[size];
			_nodeCount = 0;
		}

		/// <summary>
		/// Adds a new node
		/// </summary>
		/// <returns>index of the new node</returns>
		public int AddVertex()
		{
			// note: this method cannot add nodes beyond the initial size defined in the constructor.
			var node = new Node(_nodeCount++);
			_nodes[node.Index] = node;
			return node.Index;
		}

		/// <summary>
		/// Adds an edge from the node with the given sourceNodeIndex to the node with the given destinationNodeIndex
		/// </summary>
		/// <param name="sourceNodeIndex">index of a previously added node that is the source of the edge</param>
		/// <param name="destinationNodeIndex">index of a previously added node the is the destination of the edge</param>
		public void AddEdge(int sourceNodeIndex, int destinationNodeIndex)
		{
			// note: invalid values for "sourceNodeIndex" and "destinationNodeIndex" will either lead to an "IndexOutOfRangeException" or a "NullReferenceException"
			_nodes[sourceNodeIndex].RegisterSuccessor(_nodes[destinationNodeIndex]);
		}

		void EliminateNode(Node node)
		{
			node.Eliminate();

			// decrease _nodeCount whenever the last nodes are already eliminated.
			// This should save time for the following checks in "getNodeWithoutSuccessors".
			while (_nodeCount > 0 && _nodes[_nodeCount - 1].Eliminated)
				_nodeCount--;
		}

		Node GetNodeWithoutSuccessors()
		{
			// note: Check the nodes in reverse order, since that allows decreases of "_nodeCount" in "eliminateNode"
			// as often as possible because high indices are preferred over low indices whenever there is a choice.
			for (int i = _nodeCount - 1; i >= 0; i--)
			{
				var node = _nodes[i];
				if (node.Eliminated)
					continue;

				if (node.SuccessorCount > 0)
					continue;

				return node;
			}

			throw new Exception("Unable to find node without successors: Graph has cycles");
		}

		/// <summary>
		/// Performs the topological sort and returns an array containing the node keys in a topological order
		/// </summary>
		/// <returns>Array of node keys</returns>
		public int[] Sort()
		{
			var sortedArray = new int[_nodes.Length];

			// fill the array back to front as we select nodes without successors.
			for (int i = _nodeCount - 1; i >= 0; i--)
			{
				var node = GetNodeWithoutSuccessors();
				sortedArray[i] = node.Index;
				EliminateNode(node);
			}

			return sortedArray;
		}
	}
}
