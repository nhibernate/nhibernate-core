using System;

namespace NHibernate.Test.TypedManyToOne
{
    [Serializable]
    public class Address 
    {	
	    public virtual AddressId AddressId {get; set;}
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
