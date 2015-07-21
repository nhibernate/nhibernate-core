namespace NHibernate.Test.NHSpecificTest.NH1642
{
    public abstract class AbstractRule
    {
        public virtual int id { get; set; }
        public virtual string name { get; set; }
        public virtual string description { get; set; }
    }
}