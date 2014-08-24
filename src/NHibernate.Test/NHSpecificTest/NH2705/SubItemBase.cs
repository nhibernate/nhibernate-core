namespace NHibernate.Test.NHSpecificTest.NH2705
{
	// NOTE: an Entity and a Component in the same hierarchy is not supported
	// we are using this trick just to ""simplify"" the test.
	public class SubItemBase
	{
		public virtual string Name { get; set; }
		public virtual SubItemDetails Details { get; set; }
	}

	public class SubItemComponent : SubItemBase {}
}