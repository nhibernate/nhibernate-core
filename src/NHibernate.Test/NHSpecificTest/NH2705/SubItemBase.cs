namespace NHibernate.Test.NHSpecificTest.NH2705
{
	public class SubItemBase
	{
		public virtual string Name { get; set; }
		public virtual SubItemDetails Details { get; set; }
	}

	public class SubItemComponent : SubItemBase {}

	public class SubItemEntity : SubItemBase
	{
		public virtual int Id { get; set; }
	}
}