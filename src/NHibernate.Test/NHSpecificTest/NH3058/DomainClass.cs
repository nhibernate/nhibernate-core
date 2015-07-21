namespace NHibernate.Test.NHSpecificTest.NH3058
{
	public class DomainClass
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string ALotOfText { get; set; }

		public virtual string LoadLazyProperty()
		{
			return ALotOfText;
		}
	}
}