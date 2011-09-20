using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.FetchAllProperties
{
    [TestFixture]
    public class FetchAllPropertiesTests : TestCase
    {
        #region Overrides of TestCase

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get
            {
                return new[]
                             {
                                 "Criteria.FetchAllProperties.BlogPost.hbm.xml"
                             };
            }
        }

        protected override void OnSetUp()
        {
            using (ISession session = OpenSession())
            {
                ITransaction t = session.BeginTransaction();

                BlogPost blogPost = new BlogPost();
                blogPost.Title = "ICriteria supports eager property fetch!";
                blogPost.Description = "It really does!";
                session.Save(blogPost);

                t.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = sessions.OpenSession())
            {
                session.Delete("from System.Object");
                session.Flush();
            }
        }

        #endregion

        [Test]
        public void DidLoadDescription()
        {
            BlogPost result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(BlogPost))
                    .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .UniqueResult<BlogPost>();
                
                
            }

            Assert.DoesNotThrow(
                delegate
                    {
                        Assert.That(String.IsNullOrEmpty(result.Description), Is.False, "Should have been a value in Description");
                    }, "Should not have thrown an exception accessing the Description property");
        }

        [Test]
        public void ShouldThrowExceptionWhenAccessingLazyPropertyOutsideSession()
        {
            BlogPost result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(BlogPost))
                    .UniqueResult<BlogPost>();
                
                
            }

            Assert.Throws<LazyInitializationException>(
                delegate
                    {
                        Assume.That(String.IsNullOrEmpty(result.Description), Is.True);
                    }, "Should have thrown an exception accessing the Description property");
        }
    }
}
