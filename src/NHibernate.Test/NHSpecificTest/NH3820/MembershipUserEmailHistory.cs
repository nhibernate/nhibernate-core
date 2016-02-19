namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;

    #endregion

    public class MembershipUserEmailHistory
    {
        public MembershipUserEmailHistory(DateTime createDateTime, string oldEmail, MembershipUser user) : this()
        {
            this.CreateDateTime = createDateTime;
            this.OldEmail = oldEmail;
            this.User = user;
        }

        public MembershipUserEmailHistory()
        {
        }

        public virtual DateTime CreateDateTime { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual string OldEmail { get; protected set; }

        public virtual MembershipUser User { get; protected set; }
    }
}
