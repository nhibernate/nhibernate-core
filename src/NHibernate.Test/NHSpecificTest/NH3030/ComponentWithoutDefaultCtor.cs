namespace NHibernate.Test.NHSpecificTest.NH3030
{
    public class ComponentWithoutDefaultCtor
    {
        private string currency;
        private decimal amount;

        public ComponentWithoutDefaultCtor(string currency, decimal amount)
        {
            this.currency = currency;
            this.amount = amount;
        }

        public string Currency
        {
            get { return currency; }
        }

        public decimal Amount
        {
            get { return amount; }
        }
    }
}