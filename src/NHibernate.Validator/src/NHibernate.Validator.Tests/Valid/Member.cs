namespace NHibernate.Validator.Tests.Valid
{
    public class Member
    {
        private Address _address;

        [Valid]
        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }
    }
}