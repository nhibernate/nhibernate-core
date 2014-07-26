using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ASTNode : IASTNode, ITree
	{
		private int _startIndex;
		private int _stopIndex;
		private int _childIndex;
		private IASTNode _parent;
		private readonly IToken _token;
		private List<IASTNode> _children;

	    public ASTNode()
			: this((IToken)null) {}

		public ASTNode(IToken token)
		{
			_startIndex = -1;
			_stopIndex = -1;
			_childIndex = -1;
			_token = token;
		}

		public ASTNode(ASTNode other)
		{
			_startIndex = -1;
			_stopIndex = -1;
			_childIndex = -1;
			_token = new HqlToken(other._token);
			_startIndex = other._startIndex;
			_stopIndex = other._stopIndex;
		}

		public bool IsNil
		{
			get { return _token == null; }
		}

		public int Type
		{
			get
			{
				if (_token == null)
				{
					return 0;
				}

				return _token.Type;
			}
			set
			{
				if (_token != null)
				{
					_token.Type = value;
				}
			}
		}

		public virtual string Text
		{
			get
			{
				if (_token == null)
				{
					return null;
				}

				return _token.Text;
			}
			set
			{
				if (_token != null)
				{
					_token.Text = value;
				}
			}
		}

		public IASTNode Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public int ChildCount
		{
			get
			{
				if (_children == null)
				{
					return 0;
				}
				return _children.Count;
			}
		}

		public int ChildIndex
		{
			get { return _childIndex; }
		}

		public int Line
		{
			get
			{
				if ((_token != null) && (_token.Line != 0))
				{
					return _token.Line;
				}
				if (ChildCount > 0)
				{
					return GetChild(0).Line;
				}
				return 0;
			}
		}

		public int CharPositionInLine
		{
			get 
			{
				if ((_token != null) && (_token.CharPositionInLine != 0))
				{
					return _token.CharPositionInLine;
				}
				if (ChildCount > 0)
				{
					return GetChild(0).CharPositionInLine;
				}
				return 0;
			}
		}

		public IASTNode AddChild(IASTNode child)
		{
			if (child != null)
			{
				ASTNode childNode = (ASTNode) child;

				if (childNode.IsNil)
				{
					if ((_children != null) && (_children == childNode._children))
					{
						throw new InvalidOperationException("attempt to add child list to itself");
					}

					if (childNode._children != null)
					{
						if (_children != null)
						{
							int count = childNode._children.Count;
							for (int i = 0; i < count; i++)
							{
								ASTNode tree2 = (ASTNode) childNode._children[i];
								_children.Add(tree2);
								tree2._parent = this;
								tree2._childIndex = _children.Count - 1;
							}
						}
						else
						{
							_children = childNode._children;
							FreshenParentAndChildIndexes();
						}
					}
				}
				else
				{
					if (_children == null)
					{
						_children = new List<IASTNode>();
					}
					_children.Add(childNode);
					childNode._parent = this;
					childNode._childIndex = _children.Count - 1;
				}
			}

			return child;
		}

		public IASTNode InsertChild(int index, IASTNode child)
		{
			_children.Insert(index, child);

			FreshenParentAndChildIndexes(index);
			return child;
		}

		public IASTNode AddSibling(IASTNode newSibling)
		{
			return _parent.InsertChild(this.ChildIndex + 1, newSibling);
		}

		public void RemoveChild(IASTNode child)
		{
			RemoveChild(child.ChildIndex);
		}

		public void RemoveChild(int index)
		{
			_children.RemoveAt(index);
			FreshenParentAndChildIndexes(index);
		}

		public void ClearChildren()
		{
			if (_children != null)
			{
				_children.Clear();
			}
		}

		public void AddChildren(IEnumerable<IASTNode> children)
		{
			if (_children == null)
			{
				_children = new List<IASTNode>();
			}

			int index = _children.Count;
			_children.AddRange(children);

			FreshenParentAndChildIndexes(index);
		}

		public void AddChildren(params IASTNode[] children)
		{
			if (_children == null)
			{
				_children = new List<IASTNode>();
			}

			int index = _children.Count;
			_children.AddRange(children);

			FreshenParentAndChildIndexes(index);
		}

		public IASTNode DupNode()
		{
			return new ASTNode(this);
		}

		public IASTNode NextSibling
		{
			get
			{
				if (_parent != null && _parent.ChildCount > (_childIndex + 1))
				{
					return _parent.GetChild(_childIndex + 1);
				}

				return null;
			}
            // Setter commented out 2014-07-26. I don't like it since it drops the current next sibling from
            // the tree, and the name of the property doesn't give a clear indication if it overwrites or not.
            // Better to use InsertChild() on the parent.
			//set
			//{
			//    if (_parent != null)
			//    {
			//        if (_parent.ChildCount > (ChildIndex + 1))
			//        {
			//            _parent.SetChild(ChildIndex + 1, value);
			//        }
			//        else
			//        {
			//            AddSibling(value);
			//        }
			//    }
			//    else
			//    {
			//        throw new InvalidOperationException("Trying set NextSibling without a parent.");
			//    }
			//}
		}

		public IASTNode GetChild(int index)
		{
			if (_children == null || (_children.Count - 1) < index)
			{
				return null;
			}
			return _children[index];
		}

		public IASTNode GetFirstChild()
		{
			return GetChild(0);
		}

		public void SetFirstChild(IASTNode newChild)
		{
			if (_children == null || _children.Count == 0)
			{
				AddChild(newChild);
			}
			else
			{
				SetChild(0, newChild);
			}
		}

		public void SetChild(int index, IASTNode newChild)
		{
			if ((_children == null) || _children.Count <= index)
			{
				throw new InvalidOperationException();
			}
			var childNode = (ASTNode)newChild;
			childNode.Parent = this;
			childNode._childIndex = index;
			_children[index] = childNode;
		}

		public IToken Token
		{
			get { return _token; }
		}

		public override string ToString()
		{
			if (IsNil)
			{
				return "nil";
			}
			if (Type == 0)
			{
				return "<errornode>";
			}
			if (_token == null)
			{
				return null;
			}
			return _token.Text;
		}

		public string ToStringTree()
		{
			if ((_children == null) || (_children.Count == 0))
			{
				return ToString();
			}

			StringBuilder builder = new StringBuilder();
			if (!IsNil)
			{
				builder.Append("( ");
				builder.Append(ToString());
			}

			foreach (ASTNode child in _children)
			{
				builder.Append(' ');
				builder.Append(child.ToStringTree());
			}

			if (!IsNil)
			{
				builder.Append(" )");
			}

			return builder.ToString();
		}

		public IEnumerator<IASTNode> GetEnumerator()
		{
			if (_children == null)
			{
				_children = new List<IASTNode>();
			}

			return _children.GetEnumerator();
		}

		#region ITree
		// //////////////////////////////////////////////////////////
		// ITree implementations
		// //////////////////////////////////////////////////////////

	    public bool HasAncestor(int ttype)
	    {
	        throw new NotImplementedException();
	    }

	    public ITree GetAncestor(int ttype)
	    {
	        throw new NotImplementedException();
	    }

	    public IList GetAncestors()
	    {
	        throw new NotImplementedException();
	    }

	    void ITree.FreshenParentAndChildIndexes()
		{
			FreshenParentAndChildIndexes();
		}

		ITree ITree.GetChild(int i)
		{
			return (ITree) GetChild(i);
		}

		void ITree.AddChild(ITree t)
		{
			AddChild((IASTNode) t);
		}

		void ITree.SetChild(int i, ITree t)
		{
			ASTNode node = (ASTNode) t;
			_children[i] = node;
			node.Parent = this;
			node._childIndex = i;
		}

		object ITree.DeleteChild(int i)
		{
			object node = _children[i];
			RemoveChild(i);
			return node;
		}

		void ITree.ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
		{
            if (_children != null)
            {
                _children.RemoveRange(startChildIndex, stopChildIndex - startChildIndex + 1);
            }
            if (_children == null)
            {
                _children = new List<IASTNode>(); 
            }

            IASTNode node = t as IASTNode;

            if (node != null)
            {
                _children.Insert(startChildIndex, (IASTNode)t);
            }
            else
            {
                IEnumerable list = t as IEnumerable;

                if (list != null)
                {
                    int i = 0;
                    foreach (IASTNode entry in list)
                    {
                        _children.Insert(startChildIndex + i, entry);
                        i++;
                    }
                }
            }

			FreshenParentAndChildIndexes(startChildIndex);
		}

		ITree ITree.DupNode()
		{
			return (ITree) DupNode();
		}

		int ITree.ChildIndex
		{
			get { return _childIndex; }
			set { _childIndex = value; }
		}

		ITree ITree.Parent
		{
			get { return (ITree)Parent; }
			set { Parent = (IASTNode)value; }
		}

		int ITree.TokenStartIndex
		{
			get
			{
				if ((_startIndex == -1) && (_token != null))
				{
					return _token.TokenIndex;
				}
				return _startIndex;
			}
			set { _startIndex = value; }
		}

		int ITree.TokenStopIndex
		{
			get
			{
				if ((_stopIndex == -1) && (_token != null))
				{
					return _token.TokenIndex;
				}
				return _stopIndex;
			}
			set { _stopIndex = value; }
		}
		#endregion

		#region IEnumerable
		// //////////////////////////////////////////////////////////
		// IEnumerable implementations
		// //////////////////////////////////////////////////////////

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		// //////////////////////////////////////////////////////////
		// Private methods
		// //////////////////////////////////////////////////////////

		private void FreshenParentAndChildIndexes()
		{
			FreshenParentAndChildIndexes(0);
		}

		private void FreshenParentAndChildIndexes(int offset)
		{
			for (int i = offset; i < _children.Count; i++)
			{
				ASTNode child = (ASTNode) _children[i];
				child._childIndex = i;
				child._parent = this;
			}
		}
	}
}
