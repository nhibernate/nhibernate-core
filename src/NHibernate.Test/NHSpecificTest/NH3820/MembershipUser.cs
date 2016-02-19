namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System.Collections.Generic;

    #endregion

    public class MembershipUser
    {
        public MembershipUser(int age, string email, string name)
            : this()
        {
            this.Age = age;
            this.Email = email;
            this.Name = name;
        }

        public MembershipUser()
        {
            this.Orders = new List<MembershipOrder>();
            this.Addresses = new List<MembershipUserAddress>();
            this.EmailHistories = new List<MembershipUserEmailHistory>();
            this.Phones = new List<MembershipUserPhone>();
            this.Segments = new List<MembershipUserSegment>();
            this.Baskets = new List<MembershipUserBasket>();
        }

        public virtual int Id { get; protected set; }

        public virtual int Age { get; protected set; }

        public virtual string Email { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual ICollection<MembershipOrder> Orders { get; protected set; }

        public virtual ICollection<MembershipUserPhone> Phones { get; protected set; }

        public virtual ICollection<MembershipUserSegment> Segments { get; protected set; }

        public virtual ICollection<MembershipUserBasket> Baskets { get; protected set; }

        public virtual ICollection<MembershipUserEmailHistory> EmailHistories { get; protected set; }

        public virtual ICollection<MembershipUserAddress> Addresses { get; protected set; }

        public virtual MembershipUser AddAddress(MembershipUserAddress address)
        {
            this.Addresses.Add(address);
            return this;
        }

        public virtual MembershipUser AddPhone(MembershipUserPhone phone)
        {
            this.Phones.Add(phone);
            return this;
        }

        public virtual MembershipUser AddSegment(MembershipUserSegment segment)
        {
            this.Segments.Add(segment);
            return this;
        }

        public virtual MembershipUser AddEmailHistory(MembershipUserEmailHistory emailHistory)
        {
            this.EmailHistories.Add(emailHistory);
            return this;
        }
    }
}
