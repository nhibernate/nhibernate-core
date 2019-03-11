namespace NHibernate.Test.NHSpecificTest.NH3028
{
    public class ExEmployee : Person
    {
        protected ExEmployee() :
            this(string.Empty)
        {
        }

        public ExEmployee(string name)
            : base(name)
        {
        }

        public virtual Company Company { get; protected set; }

        public virtual void HasWorkedIn(Company company)
        {
            this.Company = company;
        }
    }
}