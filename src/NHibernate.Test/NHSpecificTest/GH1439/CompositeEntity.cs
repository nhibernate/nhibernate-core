using System;

namespace NHibernate.Test.NHSpecificTest.GH1439
{
	public class CompositeEntity : IEquatable<CompositeEntity>
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string LazyProperty { get; set; }

		public bool Equals(CompositeEntity other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Id == other.Id && string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CompositeEntity);
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
