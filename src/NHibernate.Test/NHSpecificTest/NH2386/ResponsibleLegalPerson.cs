using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2386
{
    [Serializable]
    public class ResponsibleLegalPerson  {
        private Guid responsibleLegalPersonId;
        private Organisation organisation;
        //private DateTime startDate;

        protected ResponsibleLegalPerson() {}

        public ResponsibleLegalPerson(Organisation organisation) {
            if (organisation == null) {
                throw new ArgumentNullException("organisation");
            }

            this.organisation = organisation;
        }

        public virtual Guid ResponsibleLegalPersonId {
            get { return responsibleLegalPersonId; }
            protected set { responsibleLegalPersonId = value; }
        }


        public virtual Organisation Organisation {
            get { return organisation; }
            protected set { organisation = value; }
        }

        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual string Abn { get; set; }
        public virtual string Acn { get; set; }

        public virtual string Name { get; set; }

        public override int GetHashCode() {
            return responsibleLegalPersonId.GetHashCode();
        }

        public override bool Equals(object obj) {
            ResponsibleLegalPerson other = obj as ResponsibleLegalPerson;
            if (other == null) {
                return false;
            }

            if (responsibleLegalPersonId == Guid.Empty) {
                return object.ReferenceEquals(this, other);
            }
            return this.responsibleLegalPersonId == other.responsibleLegalPersonId;
        }
    }
}
