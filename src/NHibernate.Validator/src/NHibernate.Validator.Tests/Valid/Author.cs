namespace NHibernate.Validator.Tests.Valid
{
	public class Author
	{
		private Blog blog;
		private string name;

		[NotEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Valid]
		public Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}
	}
}