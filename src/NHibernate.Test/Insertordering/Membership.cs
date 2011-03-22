using System;

namespace NHibernate.Test.Insertordering
{
	public class Membership
	{
		protected Membership() {}

		public Membership(User user, Group @group) : this(user, group, DateTime.Now) {}

		public Membership(User user, Group @group, DateTime activationDate)
		{
			User = user;
			Group = group;
			ActivationDate = activationDate;
		}

		public virtual int Id { get; protected set; }
		public virtual User User { get; private set; }
		public virtual Group Group { get; private set; }
		public virtual DateTime ActivationDate { get; private set; }
	}
}