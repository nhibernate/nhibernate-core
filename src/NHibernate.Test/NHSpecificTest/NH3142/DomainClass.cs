using System.Collections.Generic;
using System;


namespace NHibernate.Test.NHSpecificTest.NH3142
{
	public class DomainParent
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }

		public virtual ICollection<DomainChild> Children { get; set; }

		public override bool Equals(object other)
		{
			var otherParent = other as DomainParent;
			if (otherParent == null)
				return false;
			return Id1 == otherParent.Id1 &&
					Id2 == otherParent.Id2;
		}

		public override int GetHashCode()
		{
			return Id1.GetHashCode() ^ Id2.GetHashCode();
		}
	}

	public class DomainChild
	{
		public virtual int Id { get; set; }
		public virtual int ParentId1 { get; set; }
		public virtual int ParentId2 { get; set; }
	}

	[Serializable]
	public class DomainParentIdentifier
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }

		public override bool Equals(object other)
		{
			var otherParent = other as DomainParentIdentifier;
			if (otherParent == null)
				return false;
			return Id1 == otherParent.Id1 &&
					Id2 == otherParent.Id2;
		}

		public override int GetHashCode()
		{
			return Id1.GetHashCode() ^ Id2.GetHashCode();
		}
	}

	public class DomainParentWithComponentId
	{
		public DomainParentWithComponentId()
		{
			Id = new DomainParentIdentifier();
		}

		public virtual DomainParentIdentifier Id { get; protected set; }

		public virtual ICollection<DomainChildWCId> Children { get; set; }
	}

	public class DomainChildWCId
	{
		public virtual int Id { get; set; }
		public virtual int ParentId1 { get; set; }
		public virtual int ParentId2 { get; set; }
	}
}