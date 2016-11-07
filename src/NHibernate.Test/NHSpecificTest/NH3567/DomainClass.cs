

namespace NHibernate.Test.NHSpecificTest.NH3567
{
	public class Site
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }


	}
	public class Post
	{
		public virtual int Id { get; set; }
		public virtual string Content { get; set; }
		public virtual Site Site { get; set; }


	}

	public class Comment
	{
		public virtual int Id { get; set; }
		public virtual string Content { get; set; }
		public virtual Post Post { get; set; }

	}

}