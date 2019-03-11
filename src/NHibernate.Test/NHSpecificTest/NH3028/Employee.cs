namespace NHibernate.Test.NHSpecificTest.NH3028
{
    public class Employee : Person
    {
        protected Employee() :
            this(string.Empty)
        {
        }


        public Employee(string name)
            : base(name)
        {
        }

        public virtual Company Company { get; protected set; }

        public virtual void WorksIn(Company company)
        {
            this.Company = company;
        }
    }
}