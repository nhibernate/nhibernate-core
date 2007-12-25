namespace NHibernate.Validator.Tests.Valid
{
	public class Blog
	{
		private Author author;
		private string title;

		[NotEmpty]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[Valid]
		public Author Author
		{
			get { return author; }
			set { author = value; }
		}
	}
}