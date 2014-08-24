namespace NHibernate.Test.Events.Collections.Association.Bidirectional.OneToMany
{
	public class ChildWithManyToOne : ChildEntity
	{
		private IParentWithCollection parent;

		public ChildWithManyToOne() {}
		public ChildWithManyToOne(string name) : base(name) {}

		public virtual IParentWithCollection Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}