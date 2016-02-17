#region using

using System.Collections.Generic;

#endregion

namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class MembershipUser
    {
        public MembershipUser(int age, string email, string name) : this()
        {
            Age = age;
            Email = email;
            Name = name;
        }

        public MembershipUser()
        {
            Orders = new List<MembershipOrder>();
        }

        public virtual int Age { get; protected set; }

        public virtual string Email { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual ICollection<MembershipOrder> Orders { get; protected set; }

        public virtual void AddOrder(MembershipOrder order)
        {
            Orders.Add(order);
        }
    }
}
