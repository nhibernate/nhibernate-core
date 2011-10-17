namespace NHibernate.Test.NHSpecificTest.NH2846
{
    public class Post {
        public virtual int Id { get; set; }

        public virtual string Title { get; set; }

        public virtual Category Category { get; set; }
    }

    public class Category {
        public virtual int Id { get; set; }

        public virtual string Title { get; set; }
    }
}