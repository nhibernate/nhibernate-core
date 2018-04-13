# Example: Weblog Application

# Persistent Classes <a name="example-weblog-classes"></a>

The persistent classes represent a weblog, and an item posted in a
weblog. They are to be modelled as a standard parent/child relationship,
but we will use an ordered bag, instead of a set.

```csharp
    using System;
    using System.Collections.Generic;
    
    namespace Eg
    {
        public class Blog
        {
            public virtual long Id { get; set;}
    
            public virtual IList<BlogItem> Items { get; set;}
    
            public virtual string Name { get; set;}
        }
    }

    using System;
    
    namespace Eg
    {
        public class BlogItem
        {
            public virtual Blog Blog { get; set;}
    
            public virtual DateTime DateTime { get; set;}
    
            public virtual long Id { get; set;}
    
            public virtual string Text { get; set;}
    
            public virtual string Title { get; set;}
        }
    }
```

# NHibernate Mappings <a name="example-weblog-mappings"></a>

The XML mappings should now be quite straightforward.

```xml
    <?xml version="1.0" encoding="utf-8"?>
    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2" assembly="Eg" namespace="Eg">
    
        <class 
            name="Blog" 
            table="BLOGS" 
            lazy="true">
            
            <id 
                name="Id" 
                column="BLOG_ID">
                
                <generator class="native"/>
                
            </id>
            
            <property 
                name="Name" 
                column="NAME" 
                not-null="true" 
                unique="true"/>
                
            <bag
                name="Items" 
                inverse="true" 
                lazy="true"
                order-by="DATE_TIME" 
                cascade="all">
                
                <key column="BLOG_ID"/>
                <one-to-many class="BlogItem"/>
                
            </bag>
            
        </class>
        
    </hibernate-mapping>

    <?xml version="1.0" encoding="utf-8"?>
    <hibernate-mapping xmlns="urn:nhibernate-mapping-2.2" assembly="Eg" namespace="Eg">
        
        <class 
            name="BlogItem" 
            table="BLOG_ITEMS" 
            dynamic-update="true">
            
            <id 
                name="Id" 
                column="BLOG_ITEM_ID">
                
                <generator class="native"/>
                
            </id>
            
            <property 
                name="Title" 
                column="TITLE" 
                not-null="true"/>
                
            <property 
                name="Text" 
                column="TEXT" 
                not-null="true"/>
                
            <property 
                name="DateTime" 
                column="DATE_TIME" 
                not-null="true"/>
                
            <many-to-one 
                name="Blog" 
                column="BLOG_ID" 
                not-null="true"/>
                
        </class>
        
    </hibernate-mapping>
```

# NHibernate Code <a name="example-weblog-code"></a>

The following class demonstrates some of the kinds of things we can do
with these classes, using NHibernate.

```csharp
    using System;
    using System.Collections.Generic;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    
    namespace Eg
    {
        public class BlogMain
        {
            private ISessionFactory _sessions;
    
            public void Configure()
            {
                _sessions = new Configuration().Configure()
                    .BuildSessionFactory();
            }
    
            public void ExportTables()
            {
                var cfg = new Configuration().Configure();
                new SchemaExport(cfg).Create(true, true);
            }
    
            public Blog CreateBlog(string name)
            {
                var blog = new Blog
                {
                    Name = name,
                    Items = new List<BlogItem>()
                };
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    session.Save(blog);
                    tx.Commit();
                }
    
                return blog;
            }
    
            public BlogItem CreateBlogItem(Blog blog, string title, string text)
            {
                var item = new BlogItem
                {
                    Title = title,
                    Text = text,
                    Blog = blog,
                    DateTime = DateTime.Now
                };
                blog.Items.Add(item);
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    session.Update(blog);
                    tx.Commit();
                }
    
                return item;
            }
    
            public BlogItem CreateBlogItem(long blogId, string title, string text)
            {
                var item = new BlogItem
                {
                    Title = title,
                    Text = text,
                    DateTime = DateTime.Now
                };
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var blog = session.Load<Blog>(blogId);
                    item.Blog = blog;
                    blog.Items.Add(item);
                    tx.Commit();
                }
    
                return item;
            }
    
            public void UpdateBlogItem(BlogItem item, string text)
            {
                item.Text = text;
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    session.Update(item);
                    tx.Commit();
                }
            }
    
            public void UpdateBlogItem(long itemId, string text)
            {
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var item = session.Load<BlogItem>(itemId);
                    item.Text = text;
                    tx.Commit();
                }
            }
    
            public IList<object[]> ListAllBlogNamesAndItemCounts(int max)
            {
                IList<object[]> result;
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var q = session.CreateQuery(
                        "select blog.id, blog.Name, count(blogItem) " +
                        "from Blog as blog " +
                        "left outer join blog.Items as blogItem " +
                        "group by blog.Name, blog.id " +
                        "order by max(blogItem.DateTime)"
                    );
                    q.SetMaxResults(max);
                    result = q.List<object[]>();
                    tx.Commit();
                }
    
                return result;
            }
    
            public Blog GetBlogAndAllItems(long blogId)
            {
                Blog blog = null;
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var q = session.CreateQuery(
                        "from Blog as blog " +
                        "left outer join fetch blog.Items " +
                        "where blog.id = :blogId"
                    );
                    q.SetParameter("blogId", blogId);
                    blog = q.UniqueResult<Blog>();
                    tx.Commit();
                }
    
                return blog;
            }
    
            public IList<object[]> ListBlogsAndRecentItems()
            {
                IList<object[]> result = null;
    
                using (var session = _sessions.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var q = session.CreateQuery(
                        "from Blog as blog " +
                        "inner join blog.Items as blogItem " +
                        "where blogItem.DateTime > :minDate"
                    );
    
                    var date = DateTime.Now.AddMonths(-1);
                    q.SetDateTime("minDate", date);
    
                    result = q.List<object[]>();
                    tx.Commit();
                }
    
                return result;
            }
        }
    }
```

It requires some configuration settings in `web.config`, such as:

```xml
    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
      <!-- Add this element -->
      <configSections>
        <section
            name="hibernate-configuration"
            type="NHibernate.Cfg.ConfigurationSectionHandler, NHibernate" />
      </configSections>
    
      <!-- Add this element -->
      <hibernate-configuration xmlns="urn:nhibernate-configuration-2.2">
        <session-factory>
          <property name="dialect">NHibernate.Dialect.MsSql2012Dialect</property>
          <property name="connection.connection_string">
            Server=localhost\SQLEXPRESS;initial catalog=Eg;Integrated Security=True
          </property>
    
          <mapping assembly="Eg" />
        </session-factory>
      </hibernate-configuration>
    
      <!-- Leave the other sections unchanged -->
      <system.web>
        ...
      </system.web>
    </configuration>
```
