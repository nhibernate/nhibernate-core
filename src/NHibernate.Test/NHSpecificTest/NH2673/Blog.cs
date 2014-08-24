using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2673
{
	public class Blog
	{
		public Blog()
		{
			Posts = new HashSet<Post>();
			Comments = new HashSet<Comment>();
		}

		public virtual int Id { get; set; }
		public virtual string Author { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Post> Posts { get; set; }
		public virtual ISet<Comment> Comments { get; set; }
	}

	public class Post
	{
		public virtual int Id { get; protected set; }
		public virtual string Title { get; set; }
		public virtual string Body { get; set; }
	}


	public class Comment
	{
		public virtual int Id { get; protected set; }
		public virtual string Title { get; set; }
		public virtual string Body { get; set; }
	}
}