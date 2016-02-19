namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System;

    #endregion

    public class MembershipUserSegment
    {
        public MembershipUserSegment(string segmentName, DateTime createDateTime, MembershipUser user) : this()
        {
            this.SegmentName = segmentName;
            this.CreateDateTime = createDateTime;
            this.User = user;
        }

        public MembershipUserSegment()
        {
        }

        public virtual DateTime CreateDateTime { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual string SegmentName { get; protected set; }

        public virtual MembershipUser User { get; protected set; }
    }
}
