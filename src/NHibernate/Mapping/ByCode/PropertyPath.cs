using System;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	/// <summary>
	/// Immutable value class. By-value equality.
	/// </summary>
	public class PropertyPath
	{
		private readonly int hashCode;
		private readonly MemberInfo localMember;
		private readonly PropertyPath previousPath;

		public PropertyPath(PropertyPath previousPath, MemberInfo localMember)
		{
			if (localMember == null)
			{
				throw new ArgumentNullException("localMember");
			}
			this.previousPath = previousPath;
			this.localMember = localMember;
			hashCode = localMember.GetHashCode() ^ (previousPath != null ? previousPath.GetHashCode() : 41);
		}

		public PropertyPath PreviousPath
		{
			get { return previousPath; }
		}

		public MemberInfo LocalMember
		{
			get { return localMember; }
		}

		public MemberInfo GetRootMember()
		{
			PropertyPath analizing = this;
			while (analizing.previousPath != null)
			{
				analizing = analizing.previousPath;
			}
			return analizing.localMember;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj as PropertyPath);
		}

		public bool Equals(PropertyPath other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(previousPath, other.previousPath) && localMember.Equals(other.localMember);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public string ToColumnName()
		{
			return PreviousPath == null ? LocalMember.Name : PreviousPath.ToColumnName() + LocalMember.Name;
		}

		public override string ToString()
		{
			return PreviousPath == null ? LocalMember.Name : PreviousPath + "." + LocalMember.Name;
		}
	}
}