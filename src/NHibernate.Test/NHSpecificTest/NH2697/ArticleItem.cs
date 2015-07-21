

using System;
namespace NHibernate.Test.NHSpecificTest.NH2697

{
    public class ArticleItem
    {
		public virtual int Articleid { get; set; }
		public virtual Int16 IsFavorite { get; set; }
		public virtual string Name { get; set; }
		public virtual ArticleGroupItem Articlegroup { get; set; }

    }
}