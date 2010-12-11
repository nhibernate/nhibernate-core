using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2386
{
    public class Organisation
    {
        //internal to TGA
        //private int organisationId;
        public virtual Guid OrganisationId { get; set; }
        private ISet<TradingName> tradingNames;
        private ISet<ResponsibleLegalPerson> responsibleLegalPersons;

        /// <summary>
        /// 
        /// </summary>
        

         public virtual ISet<ResponsibleLegalPerson> ResponsibleLegalPersons {
            get {
                if (responsibleLegalPersons == null) {
                    responsibleLegalPersons = new HashedSet<ResponsibleLegalPerson>();
                }
                return responsibleLegalPersons;
            }
            protected set {
                responsibleLegalPersons = value;
               
            }
        }

        public virtual ISet<TradingName> TradingNames {
            get {
                if (tradingNames == null) {
                    tradingNames = new HashedSet<TradingName>();
                }
                return tradingNames;
            }
            protected set {
                tradingNames = value;
               
            }
        }

         protected internal virtual byte[] RowVersion { get; protected set; }

    }

}
