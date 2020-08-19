using System;

namespace NHibernate.Test.Extralazy
{
	public class UserFollower : IEquatable<UserFollower>
	{
		public UserFollower(User user, User follower)
		{
			User = user;
			Follower = follower;
		}

		protected UserFollower()
		{
		}

		public virtual int Id { get; set; }

		public virtual User User { get; set; }

		public virtual User Follower { get; set; }

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((UserFollower) obj);
		}

		public virtual bool Equals(UserFollower other)
		{
			if (other == null) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(User.Name, other.User.Name) && Equals(Follower.Name, other.Follower.Name);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (User.Name.GetHashCode() * 397) ^ Follower.Name.GetHashCode();
			}
		}
	}
}
