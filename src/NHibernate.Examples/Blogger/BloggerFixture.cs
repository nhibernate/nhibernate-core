using System;
using System.Collections;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Examples.Blogger 
{

	/// <summary>
	/// Summary description for BloggerFixture.
	/// </summary>
	[TestFixture]
	public class BloggerFixture 
	{

		private ISessionFactory _sessions; 

		[Test]
		public void Main() 
		{
			Configure();
			ExportTables();
			Blog blog = CreateBlog("GregBlog");
			Assert.IsTrue( blog.Id!=0, "The Id should have been assigned by the native generator" );
			
			BlogItem blogItem = CreateBlogItem(blog, "new item", "Required introduction in every blog.");
			Assert.IsTrue( blogItem.Id!=0, "The Id should have been assigned by the native generator" ); 
			Assert.AreEqual( blog, blogItem.ParentBlog, "The BlogItem's Parent should be the Blog" );
			Assert.AreEqual( "new item", blogItem.Title, "correct title was saved" );
			Assert.AreEqual( "Required introduction in every blog.", blogItem.Text, "correct text was saved." );

			BlogItem blogItem2 = CreateBlogItem(blog.Id, "item 2", "Finally some cool stuff in my second blog.");
			Assert.IsTrue( blogItem2.Id!=0, "The Id should have been assigned by the native generator." );
			Assert.AreEqual( "item 2", blogItem2.Title, "correct title was saved" );
			Assert.AreEqual( "Finally some cool stuff in my second blog.", blogItem2.Text, "correct text was saved." );
			
			UpdateBlogItem(blogItem2.Id, "Updated the cool stuff in my blog");
			
			// reload the blog to verify the db has the correct values
			ISession s = _sessions.OpenSession();
			blog = (Blog) s.Find( "from Blog as b where b.Name=:name", "GregBlog", NHibernateUtil.String )[0];
			Assert.IsNotNull(blog);
			Assert.AreEqual( 2, blog.Items.Count );

			DateTime previousItemsDate = DateTime.Parse("2004-01-01");
			int index = 0;
			
			foreach(BlogItem item in blog.Items) 
			{
				if( index > 0 ) 
				{
					Assert.IsTrue( item.ItemDate > previousItemsDate, "is order-by working" );
				}
				previousItemsDate = item.ItemDate;

				if(item.Id==blogItem.Id)  
				{
					// it is problematic to compare DateTime values when using DateTime.Now to
					// set the value because Ms Sql can _NOT_ store dates with the same accuracy
					// as the DateTime struct can - so it is possible to store a DateTime and not
					// retrieve the exact same DateTime.  See the SQL Server BOL for a description
					// of how accurately a datetime value is stored in the db.
					//Assert.AreEqual( blogItem.ItemDate, item.ItemDate );
					Assert.AreEqual( blogItem.Text, item.Text );
					Assert.AreEqual( blogItem.Title, item.Title );

					// note - we can't compare the blogItem.ParentBlog and item.ParentBlog because they
					// will be differnt objects since they were loaded in different sessions.  If Blog 
					// implemented Equals() and GetHashCode() correctly then we could compare them - they
					// would be equal by object equality - blog.Equals(otherBlog), but not have 
					// by object identity - blog==Otherblog - because they would be seperate objects in memory.
					// NHibernate guarantees the same object by identity inside of a Session only.
					Assert.IsFalse( blogItem.ParentBlog.Equals(item.ParentBlog)
						, "The ParentBlog's are not Equal because Blog does not override Equals() so it uses Object.Equals() which is comparing by object identity." );

					// The only way to make sure these are referring to the same blog is by checking for
					// database identity equality - ie: comparing there property "Id" because that is the
					// property used as the primary keys
					Assert.AreEqual( blogItem.ParentBlog.Id, item.ParentBlog.Id, "item's parent blog should have equality by database identity" );
				}

				else if( item.Id==blogItem2.Id) 
				{
					// blogItem2 has not been updated - the Id was used to load a new blogItem 
					// and update that.  So compare by what we expect it to be.
					Assert.AreEqual( "Updated the cool stuff in my blog", item.Text );
					Assert.AreEqual( "item 2", item.Title );

					// the Ids of the ParentBlog should be the same because the blog item was not moved 
					// to a different blog - so it shoud have database identity equality
					Assert.AreEqual( blog.Id, item.ParentBlog.Id );
				}
				
			}
		}

		public void Configure()
		{
			Configuration cfg = new Configuration();
			cfg.AddClass(typeof(Blog));
			cfg.AddClass(typeof(BlogItem));
			_sessions = cfg.BuildSessionFactory();
		}

		public void ExportTables()
		{
			Configuration cfg = new Configuration();
			cfg.AddClass(typeof(Blog));
			cfg.AddClass(typeof(BlogItem));
			new SchemaExport (cfg).Create(true,true);
		}

		public Blog CreateBlog(String name)
		{
			Blog blog = new Blog();
			blog.Name=name;
			blog.Items = new ArrayList();

			ISession session = _sessions.OpenSession();

			ITransaction tx = null;

			try
			{
				tx = session.BeginTransaction();
				session.Save(blog);
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx!=null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}

			return blog;
		}

		public BlogItem CreateBlogItem(Blog blog, string title, string text)
		{
			BlogItem item = new BlogItem();
			item.Title = title;
			item.Text = text;
			item.ParentBlog = blog;
			item.ItemDate = DateTime.Now;
			blog.Items.Add(item);

			ISession session = _sessions.OpenSession();
			ITransaction tx = null;

			try
			{
				tx = session.BeginTransaction();
				session.Update(blog);
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx!=null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}
			return item;
		}

		public BlogItem CreateBlogItem(long blogid, string title, string text)
		{
			BlogItem item = new BlogItem();
			item.Title = title;
			item.Text = text;
			item.ItemDate = DateTime.Now;

			ISession session = _sessions.OpenSession();
			ITransaction tx = null;
			try
			{
				tx = session.BeginTransaction();
				Blog blog = (Blog) session.Load(typeof(Blog), blogid);
				item.ParentBlog = blog;
				blog.Items.Add(item);
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx!=null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}
			return item;
		}

		public void UpdateBlogItem(long itemid, string text)
		{
			ISession session = _sessions.OpenSession();
			ITransaction tx = null;
			try
			{
				tx = session.BeginTransaction();
				BlogItem item = (BlogItem) session.Load(typeof(BlogItem),
					itemid);
				item.Text = text;
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx!=null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}
		}


	}
}
