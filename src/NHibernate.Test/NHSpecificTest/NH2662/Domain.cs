using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2662
{
	public class Customer
	{
		public virtual Guid Id
		{
			get;
			protected set;
		}
		public virtual Order Order
		{
			get;
			set;
		}
	}

    public class Order
    {
        public virtual Guid Id
        {
            get;
            protected set;
        }

        public virtual DateTime OrderDate
        {
            get;
            set;
        }
    }

    public class PizzaOrder: Order
    {
        public virtual string PizzaName
        {
            get;
            set;
        }
    }
}