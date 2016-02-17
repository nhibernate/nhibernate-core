#region using

using System.Collections.Generic;

#endregion

namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipOrder
    {
        public MembershipOrder(MembershipUser user) : this()
        {
            User = user;
        }

        public MembershipOrder()
        {
            OrderLines = new List<MembershipOrderLine>();
        }

        public virtual int Id { get; protected set; }

        public virtual ICollection<MembershipOrderLine> OrderLines { get; protected set; }

        public virtual MembershipUser User { get; protected set; }

        public virtual void AddOrderLine(MembershipOrderLine line)
        {
            OrderLines.Add(line);
        }
    }
}
