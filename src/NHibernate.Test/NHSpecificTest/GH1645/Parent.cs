namespace NHibernate.Test.NHSpecificTest.GH1645
{
	public class Parent : EntityBase
	{
		public virtual SuperParent SuperParent { get; set; }
	}
}
