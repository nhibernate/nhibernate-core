namespace NHibernate.Test.NHSpecificTest.GH2043
{
	public class EntityAssigned
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityWithAssignedBag Parent { get; set; }
	}
}
