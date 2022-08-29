namespace NHibernate.Test.NHSpecificTest.GH1645
{
	public class SuperParent : EntityBase
	{
		public virtual Parent Parent { get; set; }
	}
}
