namespace NHibernate.Test.NHSpecificTest.GH3164
{
	public class ContentItem
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IHead Head { get; set; }
	}
}
