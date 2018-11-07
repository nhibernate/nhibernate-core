using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1759
{
	class User
	{
		public virtual string Name { get; set; }
		public virtual string Org { get; set; }

		public virtual ISet<Group> Groups { get; set; } = new HashSet<Group>();

		public virtual DateTime? DepartureDate { get; set; }
		public virtual DateSpan Hiring { get; set; }
		public virtual string MainGroupName { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is User other))
				return false;

			return other.Name == Name && other.Org == Org;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Org.GetHashCode();
		}
	}

	class Group
	{
		public virtual string Name { get; set; }
		public virtual string Org { get; set; }
		public virtual string Description { get; set; }

		public virtual ISet<User> Users { get; set; } = new HashSet<User>();
		public virtual ISet<DateSpan> DateSpans { get; set; } = new HashSet<DateSpan>();
		public virtual IDictionary<DateSpan, User> UsersByHiring { get; set; } = new Dictionary<DateSpan, User>();
		public virtual IDictionary<User, string> CommentsByUser { get; set; } = new Dictionary<User, string>();

		public override bool Equals(object obj)
		{
			if (!(obj is Group other))
				return false;

			return other.Name == Name && other.Org == Org;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Org.GetHashCode();
		}
	}

	class DateSpan
	{
		public virtual DateTime? Date1 { get; set; }
		public virtual DateTime? Date2 { get; set; }
	}
}
