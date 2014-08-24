using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2386
{
   /// <summary>
    /// represents a trading name for an organisation
    /// </summary>
    public class TradingName  {
        private Organisation organisation;
        
        public virtual Guid TradingNameId { get; protected set;}
        

        public TradingName(Organisation organisation)  {
            if (organisation == null) {
                throw new ArgumentNullException("organisation");
            }
            this.organisation = organisation;
        }

        protected TradingName()  {}

        public virtual string Name { get; set; }

        public virtual Organisation Organisation {
            get { return organisation; }
            protected set { organisation = value; }
        }

        public virtual DateTime StartDate { get; set; }
        
        public virtual DateTime? EndDate { get; set; }

        private bool ShouldSerializeEndDate() {
            return EndDate.HasValue;
        }

        public override string ToString() {
            return Name;
        }

    }

       
}
