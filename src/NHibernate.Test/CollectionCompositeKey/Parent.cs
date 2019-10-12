using System.Collections.Generic;

namespace NHibernate.Test.CollectionCompositeKey
{
	public class Parent
	{
		protected Parent() { }

		public Parent(string code, int number)
		{
			Code = code;
			Number = number;
		}

		public virtual string Code { get; set; }

		public virtual int Number { get; set; }

		public virtual string Name { get; set; }

		public virtual int ReferenceNumber { get; set; }

		public virtual string ReferenceCode { get; set; }

		public virtual IList<Child> ChildrenByCompositeId { get; set; } = new List<Child>();

		public virtual IList<Child> Children { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByForeignKeys { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByComponent { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByComponentForeignKeys { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByUniqueKey { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByUniqueKeyNoManyToOne { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByCompositeUniqueKey { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenByCompositeUniqueKeyNoManyToOne { get; set; } = new List<Child>();

		public virtual IList<Child> ChildrenNoProperties { get; set; } = new List<Child>();

		public virtual IList<GrandChild> GrandChildren { get; set; } = new List<GrandChild>();

		public override bool Equals(object obj)
		{
			if (!(obj is Parent key))
			{
				return false;
			}

			if (string.IsNullOrEmpty(Code))
			{
				return ReferenceNumber.Equals(key.ReferenceNumber) && ReferenceCode.Equals(key.ReferenceCode);
			}

			return Number.Equals(key.Number) && Code.Equals(key.Code);
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(Code))
			{
				return ReferenceNumber.GetHashCode() ^ ReferenceCode.GetHashCode();
			}

			return Number.GetHashCode() ^ Code.GetHashCode();
		}
	}
}
