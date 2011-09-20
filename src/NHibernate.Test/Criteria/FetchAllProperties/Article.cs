using Iesi.Collections.Generic;

namespace NHibernate.Test.Criteria.FetchAllProperties
{
    public class Article
    {
        public Article()
        {
            Comments = new HashedSet<Comment>();
        }

        public virtual long Id { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<Comment> Comments { get; set; }

        public virtual ArticleExtension ArticleExtension { get; set; }
    }

    public class ArticleExtension
    {
        public virtual long Id { get; set; }

        public virtual Article Article { get; set; }

        public virtual int Rating { get; set; }

        public virtual string Notes { get; set; }
    }

    public class Comment
    {
        public virtual long Id { get; set; }

        public virtual string Subject { get; set; }

        public virtual string Text { get; set; }

        public virtual Article Article { get; set; }
    }
}
