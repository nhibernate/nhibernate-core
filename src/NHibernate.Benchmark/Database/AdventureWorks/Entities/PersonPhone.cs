using System;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
    public class PersonPhone
    {
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

        public virtual Person BusinessEntity { get; set; }
        public virtual PhoneNumberType PhoneNumberType { get; set; }
    }
}
