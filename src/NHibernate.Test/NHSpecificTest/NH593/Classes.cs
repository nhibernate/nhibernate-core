using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH593
{
	public class Blog
	{
		private ISet _posts;
		private ISet _users;
		private IDictionary _attributes;

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

		public virtual IDictionary Attributes
		{
			get { return _attributes; }
			set { _attributes = value; }
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
			_attributes = new Hashtable();
			_posts = new HashedSet();
			_users = new HashedSet();
		}

		public Blog(string name)
			: this()
		{
			this.blog_name = name;
		}
	}

	public class Comment
	{
		private Comment()
		{
		}

		public Comment(string text)
			: this()
		{
			_text = text;
		}

		private int _id;
		private int _indexInPost;
		private string _text;
		private Post _post;

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
		private IList _comments;

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

		private string post_title;

		public virtual string PostTitle
		{
			get { return post_title; }
			set { post_title = value; }
		}

		private Blog _blog;

		public virtual Blog Blog
		{
			get { return _blog; }
			set { _blog = value; }
		}

		public Post()
		{
			_comments = new ArrayList();
		}

		public Post(string title)
			: this()
		{
			this.post_title = title;
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

		public User(string name)
			: this()
		{
			this._userName = name;
		}
	}
}