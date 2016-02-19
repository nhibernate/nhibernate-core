namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipUserAddress
    {
        public MembershipUserAddress(string addressName, MembershipUser user) : this()
        {
            this.AddressName = addressName;
            this.User = user;
        }

        public MembershipUserAddress()
        {
        }

        public virtual string AddressName { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual MembershipUser User { get; protected set; }
    }
}
