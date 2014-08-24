using System.Collections.Generic;

namespace NHibernate.Test.Insertordering
{
	public class User
	{
		private ISet<Membership> memberships;
		public User()
		{
			memberships = new HashSet<Membership>();
		}
		public virtual int Id { get; protected set; }
		public virtual string UserName { get; set; }
		public virtual IEnumerable<Membership> Memberships
		{
			get { return memberships; }
		}

		public virtual Membership AddMembership(Group group)
		{
			var membership = new Membership(this, group);
			memberships.Add(membership);
			return membership;
		}
	}
}