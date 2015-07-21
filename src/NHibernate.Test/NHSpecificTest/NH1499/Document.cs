namespace NHibernate.Test.NHSpecificTest.NH1499
{
	public class Document
	{
		private int id;
		private Person person;
		private string title;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual Person Person
		{
			get { return person; }
			set { person = value; }
		}
	}
}