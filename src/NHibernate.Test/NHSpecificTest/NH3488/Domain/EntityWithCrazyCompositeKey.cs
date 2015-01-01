namespace NHibernate.Test.NHSpecificTest.NH3488.Domain
{
	public class EntityWithCrazyCompositeKey
	{
		private CrazyCompositeKey id;
		private string name;

		public virtual CrazyCompositeKey Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual EntityWithCrazyCompositeKey Parent { get; set; }
	}

	public class EntityReferencingEntityWithCrazyCompositeKey
	{
		private long id;
		private string name;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual EntityWithCrazyCompositeKey Parent { get; set; }
	}
}