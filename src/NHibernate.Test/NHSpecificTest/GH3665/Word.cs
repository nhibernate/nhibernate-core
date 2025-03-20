namespace NHibernate.Test.NHSpecificTest.GH3665
{
	public class Word
	{
		public virtual int Id { get; set; }
		public virtual byte[] Content { get; set; }
		public virtual Book Parent { get; set; }
	}
}
