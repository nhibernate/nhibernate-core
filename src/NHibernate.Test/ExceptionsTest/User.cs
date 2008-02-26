using System;
using Iesi.Collections;

namespace NHibernate.Test.ExceptionsTest
{
	public class User
	{
		private long id;
		private string username;
		private ISet memberships = new HashedSet();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string UserName
		{
			get { return username; }
			set { username = value; }
		}

		public virtual ISet Memberships
		{
			get { return memberships; }
			set { memberships = value; }
		}

		public virtual void AddMembership(Group membership)
		{
			if (membership == null)
			{
				throw new ArgumentNullException("membership", "Membership to add cannot be null");
			}

			memberships.Add(membership);
			membership.Members.Add(this);
		}
	}
}
