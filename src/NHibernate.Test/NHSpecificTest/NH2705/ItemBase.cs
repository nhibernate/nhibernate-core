namespace NHibernate.Test.NHSpecificTest.NH2705
{
	public class ItemBase
	{
		public virtual int Id { get; set; }
		public virtual SubItemBase SubItem { get; set; }
	}

	public class ItemWithComponentSubItem : ItemBase {}
}