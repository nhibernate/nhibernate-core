namespace NHibernate.Test.TransformTests.MultiLevelDistinctEntityTransformer
{
    public class Passport
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public Child Child { get; set; }
    }
}