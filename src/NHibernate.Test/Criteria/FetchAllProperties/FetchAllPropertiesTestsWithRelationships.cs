using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.FetchAllProperties
{
    [TestFixture]
    public class FetchAllPropertiesTestsWithRelationships : TestCase
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
                                 "Criteria.FetchAllProperties.Article.hbm.xml"
                             };
            }
        }

        protected override void OnSetUp()
        {
            using (ISession session = OpenSession())
            {
                ITransaction t = session.BeginTransaction();

                var article = new Article
                                  {
                                      Title = "Article one",
                                      Description = "The descripition for article one",
                                  };
                article.Comments.Add(new Comment { Article = article, Subject = "Cool stuff", Text = "Cool stuff comment" });
                article.Comments.Add(new Comment { Article = article, Subject = "Could it be true?", Text = "Fetch all in ICriteria rules" });
                article.ArticleExtension = new ArticleExtension { Article = article, Rating = 3, Notes = "A few notes" };
                session.Save(article);

                var article2 = new Article
                                   {
                                       Title = "Article two",
                                       Description = "The descripition for article two",
                                   };
                article2.Comments.Add(new Comment { Article = article, Subject = "The beatles", Text = "Are a great band" });
                article2.Comments.Add(new Comment { Article = article, Subject = "See you thursday", Text = "Can't wait to grab beers at the new bar!" });
                article2.ArticleExtension = new ArticleExtension { Article = article2, Rating = 5, Notes = "I don't like notes" }; 
                session.Save(article2);

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
        public void CanEagerFetchRelatedPropertiesUsingSubcriteria()
        {
            Article result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(Article))
                    .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .SetMaxResults(1)
                    .CreateCriteria("ArticleExtension")
                        .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .UniqueResult<Article>();
            }

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.Description), Is.False, "Should have been a value in Description");
                }, "Should not have thrown an exception accessing the Description property");

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.ArticleExtension.Notes), Is.False, "Should have been a value in ArticleExtension.Notes");
                }, "Should not have thrown an exception accessing the ArticleExtension.Notes property");
        }

        [Test]
        public void CanEagerFetchRelatedPropertiesUsingAssociationPath()
        {
            Article result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(Article))
                    .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .SetLazyPropertyFetchMode("ArticleExtension", LazyPropertyFetchMode.Select)
                    .SetMaxResults(1)
                    .UniqueResult<Article>();
            }

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.Description), Is.False, "Should have been a value in Description");
                }, "Should not have thrown an exception accessing the Description property");

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.ArticleExtension.Notes), Is.False, "Should have been a value in ArticleExtension.Notes");
                }, "Should not have thrown an exception accessing the ArticleExtension.Notes property");
        }

        [Test]
        public void CanEagerFetchChildAndNotParentProperties()
        {
            Article result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(Article))
                    .SetMaxResults(1)
                    .CreateCriteria("ArticleExtension")
                        .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .UniqueResult<Article>();
            }

            Assert.Throws<LazyInitializationException>(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.Description), Is.True);
                }, "Should have thrown an exception accessing the Description property");

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.ArticleExtension.Notes), Is.False, "Should have been a value in ArticleExtension.Notes");
                }, "Should not have thrown an exception accessing the ArticleExtension.Notes property");
        }

        [Test]
        public void CanEagerFetchParentAndNotChildProperties()
        {
            Article result;
            using (ISession session = sessions.OpenSession())
            {
                result = session.CreateCriteria(typeof(Article))
                    .SetMaxResults(1)
                    .SetLazyPropertyFetchMode(LazyPropertyFetchMode.Select)
                    .CreateCriteria("ArticleExtension")
                    .UniqueResult<Article>();
            }

            Assert.DoesNotThrow(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.Description), Is.False);
                }, "Should not have thrown an exception accessing the Description property");

            Assert.Throws<LazyInitializationException>(
                delegate
                {
                    Assert.That(String.IsNullOrEmpty(result.ArticleExtension.Notes), Is.True);
                }, "Should have thrown an exception accessing the ArticleExtension.Notes property");
        }
    }
}
