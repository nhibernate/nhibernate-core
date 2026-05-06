namespace NHibernate.Test.NHSpecificTest.GH3607
{
	public class LineItem
	{
		public virtual int Id { get; set; }

		public virtual Order ParentOrder { get; set; }

		public virtual string ItemName { get; set; }

		public virtual decimal Amount { get; set; }

		public virtual LineItemData Data { get; set; }
	}
}
