using System;
using Iesi.Collections;

namespace NHibernate.Test.ExceptionsTest
{
	public class Group
	{
		private long id;
		private string name;
		private ISet members;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ISet Members
		{
			get { return members; }
			set { members = value; }
		}

		public virtual void AddMember(User member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member", "Member to add cannot be null");
			}

			members.Add(member);
			member.Memberships.Add(this);
		}
	}
}
