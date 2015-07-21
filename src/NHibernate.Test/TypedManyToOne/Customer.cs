using System;

namespace NHibernate.Test.TypedManyToOne
{
    [Serializable]
	public class Customer
	{
	    public virtual string CustomerId { get; set; }
	    public virtual string Name {get; set;}
		public virtual Address BillingAddress {get; set;}
        public virtual Address ShippingAddress {get; set;}
	}
}
