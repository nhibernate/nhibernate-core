namespace NHibernate.Test.NHSpecificTest.NH2043
{
    public class A
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual B B { get; set; }
    }

    public class AImpl : A
    {
    }

    public class B
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual A A { get; set; }
    }

    public class BImpl : B
    {
    }
}
