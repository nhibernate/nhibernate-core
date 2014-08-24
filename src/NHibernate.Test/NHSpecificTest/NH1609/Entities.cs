namespace NHibernate.Test.NHSpecificTest.NH1609
{
    public class EntityA
    {
        public virtual long Id { get; set; }
    }

    public class EntityB
    {
        public virtual long Id { get; set; }
        public virtual EntityA A { get; set; }
        public virtual EntityC C { get; set; }
    }

    public class EntityC
    {
        public virtual long Id { get; set; }
    }
}
