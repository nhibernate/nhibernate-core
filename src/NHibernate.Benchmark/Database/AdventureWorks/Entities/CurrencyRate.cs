using System;
using System.Collections.Generic;

namespace NHibernate.Benchmark.Database.AdventureWorks.Entities
{
    public class CurrencyRate
    {
        public virtual int Id { get; set; }
        public virtual decimal AverageRate { get; set; }
        public virtual DateTime CurrencyRateDate { get; set; }
        public virtual decimal EndOfDayRate { get; set; }
        public virtual string FromCurrencyCode { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string ToCurrencyCode { get; set; }

        public virtual ICollection<SalesOrderHeader> SalesOrderHeader { get; set; } = new HashSet<SalesOrderHeader>();
        public virtual Currency FromCurrencyCodeNavigation { get; set; }
        public virtual Currency ToCurrencyCodeNavigation { get; set; }
    }
}
