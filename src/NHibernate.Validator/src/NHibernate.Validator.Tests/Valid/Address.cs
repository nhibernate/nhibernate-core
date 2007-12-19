namespace NHibernate.Validator.Tests.Valid
{
    public class Address
    {
        private string city;

        [NotNull]
        public string City
        {
            get { return city; }
            set { city = value; }
        }
    }
}