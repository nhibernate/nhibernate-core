using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2280
{
    public class OrganisationCode
    {
        private OrganisationCodeKey key = new OrganisationCodeKey();

        public virtual DateTime StartDate
        {
            get { return key.StartDate; }
            set { key.StartDate = value; }
        }

        public virtual string Code
        {
            get { return key.Code; }
            set { key.Code = value; }
        }

        public virtual OrganisationCodeKey Key
        {
            get { return key; }
            protected set { key = value; }
        }

        /// <summary>
        /// Need comments for intellisense.
        /// </summary>
        public virtual Organisation Organisation
        {
            get { return key.Organisation; }
            set { key.Organisation = value; }
        }


        public virtual DateTime? EndDate { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OrganisationCodeKey;
            if (other == null)
            {
                return false;
            }

            return this.Code == other.Code && this.StartDate == other.StartDate;
        }

        public override int GetHashCode()
        {
            return Code == null ? 0 : Code.GetHashCode() ^ StartDate.GetHashCode();
        }

        public override string ToString()
        {
            return Code;
        }
    }

    /// <summary>
    /// blah blah blah
    /// </summary>
    [Serializable,
     DebuggerDisplay("Organisation: {Organisation}, Code: {Code}, StartDate: {StartDate:d}")]
    public class OrganisationCodeKey
    {
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public Organisation Organisation { get; set; }

        public override bool Equals(object obj)
        {
            OrganisationCodeKey other = obj as OrganisationCodeKey;
            if (other == null)
            {
                return false;
            }

            return this.Organisation.OrganisationId == other.Organisation.OrganisationId && this.Code == other.Code &&
                   this.StartDate == other.StartDate;
        }

        public override int GetHashCode()
        {
            return (Organisation == null ? 0 : Organisation.GetHashCode()) ^ (Code == null ? 0 : Code.GetHashCode())
                   ^ StartDate.GetHashCode();
        }
    }
}
