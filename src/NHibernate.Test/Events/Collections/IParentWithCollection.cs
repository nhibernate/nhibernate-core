using System.Collections.Generic;

namespace NHibernate.Test.Events.Collections
{
	public interface IParentWithCollection : IEntity
	{
		void NewChildren(ICollection<IChild> collection);

		IChild CreateChild(string name);

		string Name { get; set; }

		ICollection<IChild> Children { get; set; }

		IChild AddChild(string childName);

		void AddChild(IChild child);

		void AddAllChildren(ICollection<IChild> children);

		void RemoveChild(IChild child);

		void RemoveAllChildren(ICollection<IChild> children);

		void ClearChildren();
	}
}