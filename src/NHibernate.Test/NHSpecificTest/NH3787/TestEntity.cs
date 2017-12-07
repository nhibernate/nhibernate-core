namespace NHibernate.Test.NHSpecificTest.NH3787
{
    public class TestEntity
    {
        public virtual int Id { get; set; }
        public virtual bool UsePreviousRate { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual decimal PreviousRate { get; set; }
    }
}
