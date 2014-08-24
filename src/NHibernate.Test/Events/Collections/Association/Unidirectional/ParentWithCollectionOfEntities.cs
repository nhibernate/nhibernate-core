namespace NHibernate.Test.Events.Collections.Association.Unidirectional
{
	public class ParentWithCollectionOfEntities : AbstractParentWithCollection
	{
		public ParentWithCollectionOfEntities() {}

		public ParentWithCollectionOfEntities(string name) : base(name) {}

		public override IChild CreateChild(string name)
		{
			return new ChildEntity(name);
		}
	}
}