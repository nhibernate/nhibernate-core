using System;

namespace NHibernate.Test.LazyOneToOne
{
	public class Employment
	{
		protected Employment() { }
		public Employment(Employee e, String org)
		{
			PersonName = e.PersonName;
			OrganizationName = org;
			StartDate = DateTime.Today.AddDays(-1);
			e.Employments.Add(this);
		}

		public virtual string PersonName { get; protected set; }

		public virtual string OrganizationName { get; protected set; }

		public virtual DateTime? StartDate { get; set; }

		public virtual DateTime? EndDate { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as Employment);
		}

		public virtual bool Equals(Employment other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.PersonName, PersonName) && Equals(other.OrganizationName, OrganizationName);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((PersonName != null ? PersonName.GetHashCode() : 0) * 397) ^ (OrganizationName != null ? OrganizationName.GetHashCode() : 0);
			}
		}
	}
}