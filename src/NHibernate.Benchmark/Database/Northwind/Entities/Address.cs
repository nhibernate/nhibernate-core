namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Address
    {
	    public Address() : this(null, null, null, null, null, null, null)
        {
        }

        public Address(string street, string city, string region, string postalCode,
                       string country, string phoneNumber, string fax)
        {
            Street = street;
            City = city;
            Region = region;
            PostalCode = postalCode;
            Country = country;
            PhoneNumber = phoneNumber;
            Fax = fax;
        }

        public string Street { get; }

        public string City { get; }

        public string Region { get; }

        public string PostalCode { get; }

        public string Country { get; }

        public string PhoneNumber { get; }

        public string Fax { get; }

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
                    Street == address.Street &&
                    City == address.City &&
                    Region == address.Region &&
                    PostalCode == address.PostalCode &&
                    Country == address.Country &&
                    PhoneNumber == address.PhoneNumber &&
                    Fax == address.Fax;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return
                (Street ?? string.Empty).GetHashCode() ^
                (City ?? string.Empty).GetHashCode() ^
                (Region ?? string.Empty).GetHashCode() ^
                (PostalCode ?? string.Empty).GetHashCode() ^
                (Country ?? string.Empty).GetHashCode() ^
                (PhoneNumber ?? string.Empty).GetHashCode() ^
                (Fax ?? string.Empty).GetHashCode();
        }
    }
}