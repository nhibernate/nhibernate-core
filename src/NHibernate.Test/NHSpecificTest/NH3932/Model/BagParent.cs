using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3932.Model
{
	public class BagParent : IParent
	{
		public BagParent()
		{
			Children = new List<Child>();
		}

		public virtual int Id { get; set; }
		public virtual int Version { get; set; }
		public virtual IList<Child> Children { get; set; }

		public virtual IParent Clone()
		{
			return new BagParent
			{
				Id = Id,
				Version = Version,
				Children = new List<Child>(Children)
			};
		}

		public virtual void ReverseChildren()
		{
			Children = new List<Child>(Children.Reverse());
		}

		public virtual void ClearChildren()
		{
			Children.Clear();
		}

		public virtual void RemoveLastChild()
		{
			Children.Remove(Children.Last());
		}

		public override bool Equals(object obj)
		{
			var that = obj as BagParent;
			return that?.Id == Id;
		}

		public override int GetHashCode()
		{
			return 0; //for simplicity - care only about equals impl
		}
	}
}