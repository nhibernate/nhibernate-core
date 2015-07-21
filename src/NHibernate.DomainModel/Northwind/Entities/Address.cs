namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Address
    {
        private readonly string _city;
        private readonly string _country;
        private readonly string _fax;
        private readonly string _phoneNumber;
        private readonly string _postalCode;
        private readonly string _region;
        private readonly string _street;

        public Address() : this(null, null, null, null, null, null, null)
        {
        }

        public Address(string street, string city, string region, string postalCode,
                       string country, string phoneNumber, string fax)
        {
            _street = street;
            _city = city;
            _region = region;
            _postalCode = postalCode;
            _country = country;
            _phoneNumber = phoneNumber;
            _fax = fax;
        }

        public string Street
        {
            get { return _street; }
        }

        public string City
        {
            get { return _city; }
        }

        public string Region
        {
            get { return _region; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
        }

        public string Country
        {
            get { return _country; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
        }

        public string Fax
        {
            get { return _fax; }
        }

        public static bool operator ==(Address address1, Address address2)
        {
            if (!ReferenceEquals(address1, null) &&
                ReferenceEquals(address2, null))
            {
                return false;
            }

            if (ReferenceEquals(address1, null) &&
                !ReferenceEquals(address2, null))
            {
                return false;
            }

            return address1.Equals(address2);
        }

        public static bool operator !=(Address address1, Address address2)
        {
            return !(address1 == address2);
        }

        public override bool Equals(object obj)
        {
            var address = obj as Address;

            if (address != null)
            {
                return
                    _street == address.Street &&
                    _city == address.City &&
                    _region == address.Region &&
                    _postalCode == address.PostalCode &&
                    _country == address.Country &&
                    _phoneNumber == address.PhoneNumber &&
                    _fax == address.Fax;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return
                (_street ?? string.Empty).GetHashCode() ^
                (_city ?? string.Empty).GetHashCode() ^
                (_region ?? string.Empty).GetHashCode() ^
                (_postalCode ?? string.Empty).GetHashCode() ^
                (_country ?? string.Empty).GetHashCode() ^
                (_phoneNumber ?? string.Empty).GetHashCode() ^
                (_fax ?? string.Empty).GetHashCode();
        }
    }
}