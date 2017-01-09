using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3932.Model
{
	public class MapParent : IParent
	{
		public MapParent()
		{
			Children = new Dictionary<int, Child>();
		}

		public virtual int Id { get; set; }
		public virtual int Version { get; set; }
		public virtual IDictionary<int, Child> Children { get; set; }

		public virtual IParent Clone()
		{
			return new MapParent
			{
				Id = Id,
				Version = Version,
				Children = new Dictionary<int, Child>(Children)
			};
		}

		public virtual void ReverseChildren()
		{
			var reversedChildren = Children.Reverse().ToList();
			Children.Clear();
			foreach (var element in reversedChildren)
			{
				Children.Add(element);
			}
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
			var that = obj as MapParent;
			return that?.Id == Id;
		}

		public override int GetHashCode()
		{
			return 0; //for simplicity - care only about equals impl
		}
	}
}