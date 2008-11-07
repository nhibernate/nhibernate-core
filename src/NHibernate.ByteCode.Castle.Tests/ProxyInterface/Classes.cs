using System.Collections;
using Iesi.Collections;

namespace NHibernate.ByteCode.Castle.Tests.ProxyInterface
{
	public class Blog
	{
		private ISet _posts;
		private ISet _users;

		private int blog_id;

		public virtual int BlogID
		{
			get { return blog_id; }
			set { blog_id = value; }
		}

		private string blog_name;

		public virtual string BlogName
		{
			get { return blog_name; }
			set { blog_name = value; }
		}

		public virtual ISet Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}

		public virtual ISet Users
		{
			get { return _users; }
			set { _users = value; }
		}

		public Blog()
		{
			_posts = new HashedSet();
			_users = new HashedSet();
		}

		public Blog(string name) : this()
		{
			blog_name = name;
		}
	}

	public class Comment
	{
		private Comment() {}

		public Comment(string text) : this()
		{
			_text = text;
		}

		private int _id;
		private int _indexInPost;
		private string _text;
		private Post _post;
		private User commenter;

		public User Commenter
		{
			get { return commenter; }
			set { commenter = value; }
		}

		public virtual int IndexInPost
		{
			get { return _indexInPost; }
			set { _indexInPost = value; }
		}

		public virtual Post Post
		{
			get { return _post; }
			set { _post = value; }
		}

		public virtual int CommentId
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Text
		{
			get { return _text; }
			set { _text = value; }
		}
	}

	public class Post
	{
		private int post_id;
		private Blog _blog;
		private string post_title;
		private IList _comments;
		private ISet categories = new HashedSet();

		public ISet Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public virtual IList Comments
		{
			get { return _comments; }
			set { _comments = value; }
		}

		public virtual int PostId
		{
			get { return post_id; }
			set { post_id = value; }
		}

		public virtual string PostTitle
		{
			get { return post_title; }
			set { post_title = value; }
		}

		public virtual Blog Blog
		{
			get { return _blog; }
			set { _blog = value; }
		}

		public Post()
		{
			_comments = new ArrayList();
		}

		public Post(string title) : this()
		{
			post_title = title;
		}
	}

	public class User
	{
		private string _userName;
		private int _userId;
		private ISet _blogs;

		public virtual ISet Blogs
		{
			get { return _blogs; }
			set { _blogs = value; }
		}

		public virtual int UserId
		{
			get { return _userId; }
			set { _userId = value; }
		}

		public virtual string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		public User()
		{
			_blogs = new HashedSet();
		}

		public User(string name) : this()
		{
			_userName = name;
		}
	}

	public class Category
	{
		private int category_id;
		private string name;
		private ISet posts = new HashedSet();

		public Category() {}

		public Category(string name)
		{
			this.name = name;
		}

		public int CategoryId
		{
			get { return category_id; }
			set { category_id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public ISet Posts
		{
			get { return posts; }
			set { posts = value; }
		}
	}
}