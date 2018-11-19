using System;

namespace NHibernate.Test.NHSpecificTest.GH1439
{
	public class EntityWithComponentId
	{
		public virtual ComponentId Id { get; set; }
		public virtual string LazyProperty { get; set; }
	}

	public class ComponentId : IEquatable<ComponentId>
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public bool Equals(ComponentId other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Id == other.Id && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ComponentId);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id * 397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}
	}
}
