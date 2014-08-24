using System.Collections.Generic;

namespace NHibernate.Test.Events.Collections
{
	public abstract class AbstractParentWithCollection : IParentWithCollection
	{
		private ICollection<IChild> children;
		private long id;
		private string name;
		protected AbstractParentWithCollection() {}

		protected AbstractParentWithCollection(string name)
		{
			this.name = name;
		}

		#region IParentWithCollection Members

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual void NewChildren(ICollection<IChild> collection)
		{
			Children = collection;
		}

		public abstract IChild CreateChild(string name);

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ICollection<IChild> Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual IChild AddChild(string childName)
		{
			IChild c = CreateChild(childName);
			AddChild(c);
			return c;
		}

		public virtual void AddChild(IChild child)
		{
			if (child != null)
			{
				children.Add(child);
			}
		}

		public virtual void AddAllChildren(ICollection<IChild> children)
		{
			foreach (IChild child in children)
			{
				this.children.Add(child);
			}
		}

		public virtual void RemoveChild(IChild child)
		{
			children.Remove(child);
		}

		public virtual void RemoveAllChildren(ICollection<IChild> children)
		{
			foreach (IChild child in children)
			{
				this.children.Remove(child);
			}
		}

		public virtual void ClearChildren()
		{
			if (children != null)
			{
				children.Clear();
			}
		}

		#endregion
	}
}