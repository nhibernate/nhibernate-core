namespace NHibernate.Test.NHSpecificTest.NH1928
{
    public class Customer 
    {
        public virtual int ID { get; private set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
    }
}
