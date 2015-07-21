namespace NHibernate.Test.Events.Collections
{
	public class ChildEntity: ChildValue, IEntity
	{
		private long id;
		public ChildEntity() {}
		public ChildEntity(string name) : base(name) {}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}