namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipUserPhone
    {
        public MembershipUserPhone(string phoneNumber, MembershipUser user) : this()
        {
            this.PhoneNumber = phoneNumber;
            this.User = user;
        }

        public MembershipUserPhone()
        {
        }

        public virtual int Id { get; protected set; }

        public virtual string PhoneNumber { get; protected set; }

        public virtual MembershipUser User { get; protected set; }
    }
}
