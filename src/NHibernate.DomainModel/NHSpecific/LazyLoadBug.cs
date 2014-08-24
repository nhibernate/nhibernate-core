using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for LLParent.
	/// </summary>
	public class LLParent
	{
		private ISet<LLChild> _children;
		private ISet<LLChildNoAdd> _childrenNoAdd;

		public ISet<LLChild> Children
		{
			get
			{
				if (_children == null)
					_children = new HashSet<LLChild>();
				return _children;
			}
			set { _children = value; }
		}

		public ISet<LLChildNoAdd> ChildrenNoAdd
		{
			get
			{
				if (_childrenNoAdd == null)
					_childrenNoAdd = new HashSet<LLChildNoAdd>();
				return _childrenNoAdd;
			}
			set { _childrenNoAdd = value; }
		}
	}

	/// <summary>
	/// Summary description for LLChild.
	/// </summary>
	public class LLChild
	{
		private LLParent _parent;

		public LLParent Parent
		{
			get { return _parent; }
			set
			{
				_parent = value;
				// this is the source of the "bug" - more accurately described as user error
				// but it raised a Null Pointer Exception instead of a LazyInitializationException
				// like it should have.  
				_parent.Children.Add(this);
			}
		}
	}

	/// <summary>
	/// Summary description for LLChild.
	/// </summary>
	public class LLChildNoAdd
	{
		private LLParent _parent;

		public LLParent Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
	}
}