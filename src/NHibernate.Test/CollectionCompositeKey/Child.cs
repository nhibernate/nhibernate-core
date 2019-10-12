
namespace NHibernate.Test.CollectionCompositeKey
{
	public class Child
	{
		protected Child()
		{
		}

		public Child(int number, string name, Parent parent)
		{
			Id = new ChildCompositeId()
			{
				Number = number,
				ParentNumber = parent.Number,
				ParentCode = parent.Code
			};
			Name = name;
		}

		public virtual ChildCompositeId Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Parent Parent { get; set; }

		public virtual Parent ParentByName { get; set; }

		public virtual string ParentName { get; set; }

		public virtual Parent ParentByReference { get; set; }

		public virtual string ParentReferenceCode { get; set; }

		public virtual int ParentReferenceNumber { get; set; }

		public virtual string ParentCode { get; set; }

		public virtual int ParentNumber { get; set; }

		public virtual ChildComponent Component { get; set; } = new ChildComponent();
	}
}
