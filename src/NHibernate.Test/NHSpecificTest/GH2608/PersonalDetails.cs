namespace NHibernate.Test.NHSpecificTest.GH2608
{
	public class PersonalDetails
	{
		public virtual long Id { get; set; }
		public virtual string SomePersonalDetail { get; set; }

		public virtual Person Person { get; set; }
	}
}
