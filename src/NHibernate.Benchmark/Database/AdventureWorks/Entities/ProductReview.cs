using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
    public class ProductReview
    {
        public virtual int Id { get; set; }
        public virtual string Comments { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual int Rating { get; set; }
        public virtual DateTime ReviewDate { get; set; }
        public virtual string ReviewerName { get; set; }
        public virtual Product Product { get; set; }
    }
}
